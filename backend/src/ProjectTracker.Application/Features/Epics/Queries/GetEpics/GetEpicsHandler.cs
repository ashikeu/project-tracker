using Microsoft.EntityFrameworkCore;
using ProjectTracker.Application.Common.Interfaces;
using ProjectTracker.Application.Features.Epics.Dtos;
using ProjectTracker.Domain.Enums;

namespace ProjectTracker.Application.Features.Epics.Queries.GetEpics;

public record GetEpicsQuery(Guid ProjectId);

public interface IGetEpicsHandler
{
    Task<IReadOnlyList<EpicDto>> Handle(GetEpicsQuery query, CancellationToken ct);
}

public class GetEpicsHandler : IGetEpicsHandler
{
    private readonly IApplicationDbContext _db;

    public GetEpicsHandler(IApplicationDbContext db) => _db = db;

    public async Task<IReadOnlyList<EpicDto>> Handle(GetEpicsQuery query, CancellationToken ct)
    {
        return await _db.Epics.AsNoTracking()
            .Where(e => e.ProjectId == query.ProjectId)
            .OrderBy(e => e.CreatedAt)
            .Select(e => new EpicDto(
                e.Id, e.ProjectId, e.Name, e.Description, e.OwnerUserId,
                e.Owner != null ? e.Owner.FirstName + " " + e.Owner.LastName : null,
                e.Status.ToString(), e.Priority.ToString(), e.Color, e.Icon,
                e.StartDate, e.EndDate,
                e.Tasks.Count, e.Tasks.Count(t => t.Status == TaskItemStatus.Done), e.CreatedAt))
            .ToListAsync(ct);
    }
}
