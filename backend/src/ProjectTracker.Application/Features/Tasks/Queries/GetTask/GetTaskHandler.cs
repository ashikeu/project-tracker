using Microsoft.EntityFrameworkCore;
using ProjectTracker.Application.Common.Interfaces;
using ProjectTracker.Application.Features.Tasks.Dtos;
using ProjectTracker.Application.Features.Tasks.Mappings;
using ProjectTracker.Shared.Results;

namespace ProjectTracker.Application.Features.Tasks.Queries.GetTask;

public record GetTaskQuery(Guid TaskId);

public interface IGetTaskHandler
{
    Task<Result<TaskDetailDto>> Handle(GetTaskQuery query, CancellationToken ct);
}

public class GetTaskHandler : IGetTaskHandler
{
    private readonly IApplicationDbContext _db;

    public GetTaskHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result<TaskDetailDto>> Handle(GetTaskQuery query, CancellationToken ct)
    {
        var task = await _db.Tasks.AsNoTracking().Where(t => t.Id == query.TaskId).Select(TaskMappings.ToDto).FirstOrDefaultAsync(ct);
        if (task is null) return Result<TaskDetailDto>.Fail("Task not found.");

        var checklist = await _db.ChecklistItems.AsNoTracking()
            .Where(c => c.TaskId == query.TaskId).OrderBy(c => c.Order)
            .Select(c => new ChecklistItemDto(c.Id, c.Text, c.IsDone, c.Order))
            .ToListAsync(ct);

        var dependencies = await _db.TaskDependencies.AsNoTracking()
            .Where(d => d.TaskId == query.TaskId)
            .Select(d => new TaskDependencyDto(d.Id, d.DependsOnTaskId, d.DependsOnTask.TaskKey, d.DependsOnTask.Title, d.Type.ToString()))
            .ToListAsync(ct);

        var watchers = await _db.TaskWatchers.AsNoTracking()
            .Where(w => w.TaskId == query.TaskId)
            .Select(w => new TaskWatcherDto(w.UserId, w.User.Email, w.User.FirstName, w.User.LastName))
            .ToListAsync(ct);

        return Result<TaskDetailDto>.Ok(new TaskDetailDto(task, checklist, dependencies, watchers));
    }
}
