using Microsoft.EntityFrameworkCore;
using ProjectTracker.Application.Common.Interfaces;
using ProjectTracker.Application.Features.Teams.Dtos;
using ProjectTracker.Shared.Results;

namespace ProjectTracker.Application.Features.Teams.Queries.GetTeams;

public record GetTeamsQuery(Guid OrganizationId, bool IncludeArchived, int Page, int PageSize);

public interface IGetTeamsHandler
{
    Task<PagedResult<TeamDto>> Handle(GetTeamsQuery query, CancellationToken ct);
}

public class GetTeamsHandler : IGetTeamsHandler
{
    private readonly IApplicationDbContext _db;

    public GetTeamsHandler(IApplicationDbContext db) => _db = db;

    public async Task<PagedResult<TeamDto>> Handle(GetTeamsQuery query, CancellationToken ct)
    {
        var baseQuery = _db.Teams.AsNoTracking()
            .Where(t => t.OrganizationId == query.OrganizationId && (query.IncludeArchived || !t.IsArchived))
            .OrderBy(t => t.Name);

        var totalCount = await baseQuery.CountAsync(ct);
        var page = Math.Max(query.Page, 1);
        var pageSize = query.PageSize <= 0 ? 50 : Math.Min(query.PageSize, 200);

        var items = await baseQuery
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(t => new TeamDto(
                t.Id, t.Name, t.Description, t.LeadUserId,
                t.Lead != null ? t.Lead.FirstName + " " + t.Lead.LastName : null,
                t.IsArchived, t.Members.Count, t.CreatedAt))
            .ToListAsync(ct);

        return new PagedResult<TeamDto>(items, page, pageSize, totalCount);
    }
}
