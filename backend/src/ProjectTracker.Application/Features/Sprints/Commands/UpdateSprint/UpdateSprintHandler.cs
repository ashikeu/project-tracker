using Microsoft.EntityFrameworkCore;
using ProjectTracker.Application.Common.Interfaces;
using ProjectTracker.Application.Features.Sprints.Dtos;
using ProjectTracker.Application.Features.Sprints.Mappings;
using ProjectTracker.Shared.Results;

namespace ProjectTracker.Application.Features.Sprints.Commands.UpdateSprint;

public interface IUpdateSprintHandler
{
    Task<Result<SprintDto>> Handle(UpdateSprintCommand command, CancellationToken ct);
}

public class UpdateSprintHandler : IUpdateSprintHandler
{
    private readonly IApplicationDbContext _db;

    public UpdateSprintHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result<SprintDto>> Handle(UpdateSprintCommand command, CancellationToken ct)
    {
        var sprint = await _db.Sprints.FirstOrDefaultAsync(s => s.Id == command.SprintId, ct);
        if (sprint is null) return Result<SprintDto>.Fail("Sprint not found.");

        sprint.Name = command.Name.Trim();
        sprint.Goal = command.Goal?.Trim();
        sprint.StartDate = command.StartDate;
        sprint.EndDate = command.EndDate;

        await _db.SaveChangesAsync(ct);

        var dto = await _db.Sprints.AsNoTracking().Where(s => s.Id == sprint.Id)
            .Select(SprintMappings.ToDto).FirstAsync(ct);
        return Result<SprintDto>.Ok(dto);
    }
}
