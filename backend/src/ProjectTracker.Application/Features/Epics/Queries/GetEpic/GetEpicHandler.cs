using Microsoft.EntityFrameworkCore;
using ProjectTracker.Application.Common.Interfaces;
using ProjectTracker.Application.Features.Epics.Dtos;
using ProjectTracker.Domain.Enums;
using ProjectTracker.Shared.Results;

namespace ProjectTracker.Application.Features.Epics.Queries.GetEpic;

public record GetEpicQuery(Guid EpicId);

public interface IGetEpicHandler
{
    Task<Result<EpicDto>> Handle(GetEpicQuery query, CancellationToken ct);
}

public class GetEpicHandler : IGetEpicHandler
{
    private readonly IApplicationDbContext _db;

    public GetEpicHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result<EpicDto>> Handle(GetEpicQuery query, CancellationToken ct)
    {
        var epic = await _db.Epics.AsNoTracking()
            .Where(e => e.Id == query.EpicId)
            .Select(e => new EpicDto(
                e.Id, e.ProjectId, e.Name, e.Description, e.OwnerUserId,
                e.Owner != null ? e.Owner.FirstName + " " + e.Owner.LastName : null,
                e.Status.ToString(), e.Priority.ToString(), e.Color, e.Icon,
                e.StartDate, e.EndDate,
                e.Tasks.Count, e.Tasks.Count(t => t.Status == TaskItemStatus.Done), e.CreatedAt))
            .FirstOrDefaultAsync(ct);

        return epic is null ? Result<EpicDto>.Fail("Epic not found.") : Result<EpicDto>.Ok(epic);
    }
}
