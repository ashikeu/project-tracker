using Microsoft.EntityFrameworkCore;
using ProjectTracker.Application.Common.Interfaces;
using ProjectTracker.Application.Features.Tasks.Dtos;
using ProjectTracker.Application.Features.Tasks.Mappings;
using ProjectTracker.Shared.Results;

namespace ProjectTracker.Application.Features.Backlog.Commands.MoveBacklogItem;

/// <summary>TargetIndex is the desired 0-based position among the OTHER backlog items (excluding
/// the one being moved).</summary>
public record MoveBacklogItemCommand(Guid TaskId, int TargetIndex);

public interface IMoveBacklogItemHandler
{
    Task<Result<TaskDto>> Handle(MoveBacklogItemCommand command, CancellationToken ct);
}

/// <summary>Reorders a task within the product backlog using fractional-index positioning — same
/// approach and self-healing reindex as MoveTaskHandler (see its remarks), but scoped to the
/// project's backlog (SprintId == null, top-level) instead of a board column.</summary>
public class MoveBacklogItemHandler : IMoveBacklogItemHandler
{
    private const double MinGap = 1e-6;

    private readonly IApplicationDbContext _db;

    public MoveBacklogItemHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result<TaskDto>> Handle(MoveBacklogItemCommand command, CancellationToken ct)
    {
        var task = await _db.Tasks.FirstOrDefaultAsync(t => t.Id == command.TaskId, ct);
        if (task is null) return Result<TaskDto>.Fail("Task not found.");
        if (task.SprintId is not null || task.ParentTaskId is not null)
        {
            return Result<TaskDto>.Fail("Only top-level backlog tasks (not in a sprint) can be reordered in the backlog.");
        }

        var siblingPositions = await SiblingPositionsAsync(task.ProjectId, task.Id, ct);

        var index = Math.Clamp(command.TargetIndex, 0, siblingPositions.Count);
        var lower = index == 0 ? (double?)null : siblingPositions[index - 1];
        var upper = index == siblingPositions.Count ? (double?)null : siblingPositions[index];

        if (lower is not null && upper is not null && upper.Value - lower.Value < MinGap)
        {
            await ReindexBacklogAsync(task.ProjectId, ct);
            siblingPositions = await SiblingPositionsAsync(task.ProjectId, task.Id, ct);
            index = Math.Clamp(command.TargetIndex, 0, siblingPositions.Count);
            lower = index == 0 ? null : siblingPositions[index - 1];
            upper = index == siblingPositions.Count ? null : siblingPositions[index];
        }

        task.Position = (lower, upper) switch
        {
            (null, null) => 1024.0,
            (null, not null) => upper!.Value - 1024.0,
            (not null, null) => lower!.Value + 1024.0,
            (not null, not null) => (lower!.Value + upper!.Value) / 2.0
        };

        await _db.SaveChangesAsync(ct);

        var dto = await _db.Tasks.AsNoTracking().Where(t => t.Id == task.Id).Select(TaskMappings.ToDto).FirstAsync(ct);
        return Result<TaskDto>.Ok(dto);
    }

    private Task<List<double>> SiblingPositionsAsync(Guid projectId, Guid excludeTaskId, CancellationToken ct) =>
        _db.Tasks.AsNoTracking()
            .Where(t => t.ProjectId == projectId && t.SprintId == null && t.ParentTaskId == null && t.Id != excludeTaskId)
            .OrderBy(t => t.Position)
            .Select(t => t.Position)
            .ToListAsync(ct);

    private async Task ReindexBacklogAsync(Guid projectId, CancellationToken ct)
    {
        var tasks = await _db.Tasks
            .Where(t => t.ProjectId == projectId && t.SprintId == null && t.ParentTaskId == null)
            .OrderBy(t => t.Position).ToListAsync(ct);
        for (var i = 0; i < tasks.Count; i++) tasks[i].Position = (i + 1) * 1024.0;
        await _db.SaveChangesAsync(ct);
    }
}
