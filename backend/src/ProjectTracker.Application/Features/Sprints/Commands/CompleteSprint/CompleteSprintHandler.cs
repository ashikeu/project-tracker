using Microsoft.EntityFrameworkCore;
using ProjectTracker.Application.Common.Interfaces;
using ProjectTracker.Application.Features.Sprints.Dtos;
using ProjectTracker.Application.Features.Sprints.Mappings;
using ProjectTracker.Domain.BusinessRules;
using ProjectTracker.Domain.Enums;
using ProjectTracker.Shared.Results;

namespace ProjectTracker.Application.Features.Sprints.Commands.CompleteSprint;

/// <summary>MoveIncompleteToBacklog: when true, unfinished (non-Done) tasks are returned to the
/// product backlog (SprintId cleared) as the sprint closes; when false they stay attached to the
/// completed sprint as a historical record. The client chooses — nothing is mutated silently by
/// default beyond the sprint's own status.</summary>
public record CompleteSprintCommand(Guid SprintId, bool MoveIncompleteToBacklog);

public interface ICompleteSprintHandler
{
    Task<Result<SprintDto>> Handle(CompleteSprintCommand command, CancellationToken ct);
}

public class CompleteSprintHandler : ICompleteSprintHandler
{
    private readonly IApplicationDbContext _db;

    public CompleteSprintHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result<SprintDto>> Handle(CompleteSprintCommand command, CancellationToken ct)
    {
        var sprint = await _db.Sprints.FirstOrDefaultAsync(s => s.Id == command.SprintId, ct);
        if (sprint is null) return Result<SprintDto>.Fail("Sprint not found.");

        if (!SprintStatusTransition.CanTransition(sprint.Status, SprintStatus.Completed))
        {
            return Result<SprintDto>.Fail(SprintStatusTransition.DescribeInvalid(sprint.Status, SprintStatus.Completed));
        }

        if (command.MoveIncompleteToBacklog)
        {
            var incomplete = await _db.Tasks
                .Where(t => t.SprintId == sprint.Id && t.Status != TaskItemStatus.Done)
                .ToListAsync(ct);
            foreach (var task in incomplete) task.SprintId = null;
        }

        sprint.Status = SprintStatus.Completed;
        sprint.CompletedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);

        var dto = await _db.Sprints.AsNoTracking().Where(s => s.Id == sprint.Id)
            .Select(SprintMappings.ToDto).FirstAsync(ct);
        return Result<SprintDto>.Ok(dto, "Sprint completed.");
    }
}
