using Microsoft.EntityFrameworkCore;
using ProjectTracker.Application.Common.Interfaces;
using ProjectTracker.Application.Features.Sprints.Dtos;
using ProjectTracker.Application.Features.Sprints.Mappings;

namespace ProjectTracker.Application.Features.Sprints.Queries.GetSprints;

public interface IGetSprintsHandler
{
    Task<IReadOnlyList<SprintDto>> Handle(GetSprintsQuery query, CancellationToken ct);
}

public class GetSprintsHandler : IGetSprintsHandler
{
    private readonly IApplicationDbContext _db;

    public GetSprintsHandler(IApplicationDbContext db) => _db = db;

    public async Task<IReadOnlyList<SprintDto>> Handle(GetSprintsQuery query, CancellationToken ct)
    {
        var q = _db.Sprints.AsNoTracking().Where(s => s.ProjectId == query.ProjectId);
        if (query.Status is not null) q = q.Where(s => s.Status == query.Status);

        return await q.OrderBy(s => s.CreatedAt).Select(SprintMappings.ToDto).ToListAsync(ct);
    }
}
