using Microsoft.EntityFrameworkCore;
using ProjectTracker.Application.Common.Interfaces;
using ProjectTracker.Application.Features.Sprints.Dtos;
using ProjectTracker.Application.Features.Sprints.Mappings;
using ProjectTracker.Domain.BusinessRules;
using ProjectTracker.Domain.Enums;
using ProjectTracker.Shared.Results;

namespace ProjectTracker.Application.Features.Sprints.Commands.StartSprint;

public record StartSprintCommand(Guid SprintId);

public interface IStartSprintHandler
{
    Task<Result<SprintDto>> Handle(StartSprintCommand command, CancellationToken ct);
}

public class StartSprintHandler : IStartSprintHandler
{
    private readonly IApplicationDbContext _db;

    public StartSprintHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result<SprintDto>> Handle(StartSprintCommand command, CancellationToken ct)
    {
        var sprint = await _db.Sprints.FirstOrDefaultAsync(s => s.Id == command.SprintId, ct);
        if (sprint is null) return Result<SprintDto>.Fail("Sprint not found.");

        if (!SprintStatusTransition.CanTransition(sprint.Status, SprintStatus.Active))
        {
            return Result<SprintDto>.Fail(SprintStatusTransition.DescribeInvalid(sprint.Status, SprintStatus.Active));
        }

        // Scrum invariant: at most one Active sprint per project at a time.
        var hasActive = await _db.Sprints.AsNoTracking()
            .AnyAsync(s => s.ProjectId == sprint.ProjectId && s.Id != sprint.Id && s.Status == SprintStatus.Active, ct);
        if (hasActive)
        {
            return Result<SprintDto>.Fail("This project already has an active sprint. Complete or cancel it before starting another.");
        }

        sprint.Status = SprintStatus.Active;
        await _db.SaveChangesAsync(ct);

        var dto = await _db.Sprints.AsNoTracking().Where(s => s.Id == sprint.Id)
            .Select(SprintMappings.ToDto).FirstAsync(ct);
        return Result<SprintDto>.Ok(dto, "Sprint started.");
    }
}
