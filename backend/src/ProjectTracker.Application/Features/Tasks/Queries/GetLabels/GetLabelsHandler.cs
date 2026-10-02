using Microsoft.EntityFrameworkCore;
using ProjectTracker.Application.Common.Interfaces;
using ProjectTracker.Application.Features.Tasks.Dtos;

namespace ProjectTracker.Application.Features.Tasks.Queries.GetLabels;

public record GetLabelsQuery(Guid ProjectId);

public interface IGetLabelsHandler
{
    Task<IReadOnlyList<LabelDto>> Handle(GetLabelsQuery query, CancellationToken ct);
}

public class GetLabelsHandler : IGetLabelsHandler
{
    private readonly IApplicationDbContext _db;

    public GetLabelsHandler(IApplicationDbContext db) => _db = db;

    public async Task<IReadOnlyList<LabelDto>> Handle(GetLabelsQuery query, CancellationToken ct)
    {
        return await _db.Labels.AsNoTracking()
            .Where(l => l.ProjectId == query.ProjectId)
            .OrderBy(l => l.Name)
            .Select(l => new LabelDto(l.Id, l.Name, l.Color))
            .ToListAsync(ct);
    }
}
