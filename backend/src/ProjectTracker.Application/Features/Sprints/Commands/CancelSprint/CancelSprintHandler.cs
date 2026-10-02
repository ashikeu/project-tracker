using Microsoft.EntityFrameworkCore;
using ProjectTracker.Application.Common.Interfaces;
using ProjectTracker.Application.Features.Sprints.Dtos;
using ProjectTracker.Application.Features.Sprints.Mappings;
using ProjectTracker.Domain.BusinessRules;
using ProjectTracker.Domain.Enums;
using ProjectTracker.Shared.Results;

namespace ProjectTracker.Application.Features.Sprints.Commands.CancelSprint;

/// <summary>Cancelling always returns the sprint's tasks to the product backlog (SprintId cleared)
/// — a cancelled sprint's work isn't "done", so leaving tasks stranded on a dead sprint would hide
/// them from the backlog.</summary>
public record CancelSprintCommand(Guid SprintId);

public interface ICancelSprintHandler
{
    Task<Result<SprintDto>> Handle(CancelSprintCommand command, CancellationToken ct);
}

public class CancelSprintHandler : ICancelSprintHandler
{
    private readonly IApplicationDbContext _db;

    public CancelSprintHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result<SprintDto>> Handle(CancelSprintCommand command, CancellationToken ct)
    {
        var sprint = await _db.Sprints.FirstOrDefaultAsync(s => s.Id == command.SprintId, ct);
        if (sprint is null) return Result<SprintDto>.Fail("Sprint not found.");

        if (!SprintStatusTransition.CanTransition(sprint.Status, SprintStatus.Cancelled))
        {
            return Result<SprintDto>.Fail(SprintStatusTransition.DescribeInvalid(sprint.Status, SprintStatus.Cancelled));
        }

        var attached = await _db.Tasks.Where(t => t.SprintId == sprint.Id).ToListAsync(ct);
        foreach (var task in attached) task.SprintId = null;

        sprint.Status = SprintStatus.Cancelled;
        await _db.SaveChangesAsync(ct);

        var dto = await _db.Sprints.AsNoTracking().Where(s => s.Id == sprint.Id)
            .Select(SprintMappings.ToDto).FirstAsync(ct);
        return Result<SprintDto>.Ok(dto, "Sprint cancelled.");
    }
}
