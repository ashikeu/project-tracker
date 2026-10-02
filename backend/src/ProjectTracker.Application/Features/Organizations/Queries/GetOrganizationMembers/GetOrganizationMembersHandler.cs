using Microsoft.EntityFrameworkCore;
using ProjectTracker.Application.Common.Interfaces;
using ProjectTracker.Application.Features.Organizations.Dtos;
using ProjectTracker.Shared.Results;

namespace ProjectTracker.Application.Features.Organizations.Queries.GetOrganizationMembers;

public record GetOrganizationMembersQuery(Guid OrganizationId, int Page, int PageSize);

public interface IGetOrganizationMembersHandler
{
    Task<PagedResult<OrganizationMemberDto>> Handle(GetOrganizationMembersQuery query, CancellationToken ct);
}

public class GetOrganizationMembersHandler : IGetOrganizationMembersHandler
{
    private readonly IApplicationDbContext _db;

    public GetOrganizationMembersHandler(IApplicationDbContext db) => _db = db;

    public async Task<PagedResult<OrganizationMemberDto>> Handle(GetOrganizationMembersQuery query, CancellationToken ct)
    {
        var baseQuery = _db.OrganizationUsers.AsNoTracking()
            .Where(ou => ou.OrganizationId == query.OrganizationId)
            .OrderBy(ou => ou.User.FirstName).ThenBy(ou => ou.User.LastName);

        var totalCount = await baseQuery.CountAsync(ct);

        var page = Math.Max(query.Page, 1);
        var pageSize = query.PageSize <= 0 ? 50 : Math.Min(query.PageSize, 200);

        var items = await baseQuery
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(ou => new OrganizationMemberDto(
                ou.Id, ou.UserId, ou.User.Email, ou.User.FirstName, ou.User.LastName,
                ou.User.AvatarUrl, ou.RoleId, ou.Role.Name, ou.Status.ToString(), ou.JoinedAt))
            .ToListAsync(ct);

        return new PagedResult<OrganizationMemberDto>(items, page, pageSize, totalCount);
    }
}
