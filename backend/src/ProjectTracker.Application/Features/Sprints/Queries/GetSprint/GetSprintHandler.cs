using Microsoft.EntityFrameworkCore;
using ProjectTracker.Application.Common.Interfaces;
using ProjectTracker.Application.Features.Sprints.Dtos;
using ProjectTracker.Application.Features.Sprints.Mappings;
using ProjectTracker.Shared.Results;

namespace ProjectTracker.Application.Features.Sprints.Queries.GetSprint;

public record GetSprintQuery(Guid SprintId);

public interface IGetSprintHandler
{
    Task<Result<SprintDto>> Handle(GetSprintQuery query, CancellationToken ct);
}

public class GetSprintHandler : IGetSprintHandler
{
    private readonly IApplicationDbContext _db;

    public GetSprintHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result<SprintDto>> Handle(GetSprintQuery query, CancellationToken ct)
    {
        var sprint = await _db.Sprints.AsNoTracking()
            .Where(s => s.Id == query.SprintId)
            .Select(SprintMappings.ToDto)
            .FirstOrDefaultAsync(ct);

        return sprint is null ? Result<SprintDto>.Fail("Sprint not found.") : Result<SprintDto>.Ok(sprint);
    }
}
