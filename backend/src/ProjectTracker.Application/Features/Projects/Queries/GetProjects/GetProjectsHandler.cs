using Microsoft.EntityFrameworkCore;
using ProjectTracker.Application.Common.Interfaces;
using ProjectTracker.Application.Features.Projects.Dtos;
using ProjectTracker.Domain.Enums;
using ProjectTracker.Shared.Results;

namespace ProjectTracker.Application.Features.Projects.Queries.GetProjects;

public record GetProjectsQuery(Guid OrganizationId, bool IncludeArchived, ProjectStatus? Status, string? Search, int Page, int PageSize);

public interface IGetProjectsHandler
{
    Task<PagedResult<ProjectDto>> Handle(GetProjectsQuery query, CancellationToken ct);
}

public class GetProjectsHandler : IGetProjectsHandler
{
    private readonly IApplicationDbContext _db;

    public GetProjectsHandler(IApplicationDbContext db) => _db = db;

    public async Task<PagedResult<ProjectDto>> Handle(GetProjectsQuery query, CancellationToken ct)
    {
        var baseQuery = _db.Projects.AsNoTracking()
            .Where(p => p.OrganizationId == query.OrganizationId && (query.IncludeArchived || !p.IsArchived));

        if (query.Status is not null)
        {
            baseQuery = baseQuery.Where(p => p.Status == query.Status);
        }

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var term = query.Search.Trim();
            baseQuery = baseQuery.Where(p => p.Name.Contains(term) || p.Key.Contains(term));
        }

        baseQuery = baseQuery.OrderBy(p => p.Key);

        var totalCount = await baseQuery.CountAsync(ct);
        var page = Math.Max(query.Page, 1);
        var pageSize = query.PageSize <= 0 ? 50 : Math.Min(query.PageSize, 200);

        var items = await baseQuery
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(p => new ProjectDto(
                p.Id, p.Key, p.Name, p.Description,
                p.OwnerUserId, p.Owner.FirstName + " " + p.Owner.LastName,
                p.ManagerUserId, p.Manager != null ? p.Manager.FirstName + " " + p.Manager.LastName : null,
                p.Status.ToString(), p.Priority.ToString(), p.StartDate, p.TargetDate,
                p.IsArchived, p.CreatedAt))
            .ToListAsync(ct);

        return new PagedResult<ProjectDto>(items, page, pageSize, totalCount);
    }
}
