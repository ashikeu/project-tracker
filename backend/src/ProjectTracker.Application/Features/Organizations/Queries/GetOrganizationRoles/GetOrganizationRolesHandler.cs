using Microsoft.EntityFrameworkCore;
using ProjectTracker.Application.Common.Interfaces;
using ProjectTracker.Application.Features.Organizations.Dtos;

namespace ProjectTracker.Application.Features.Organizations.Queries.GetOrganizationRoles;

public record GetOrganizationRolesQuery(Guid OrganizationId);

public interface IGetOrganizationRolesHandler
{
    Task<IReadOnlyList<RoleDto>> Handle(GetOrganizationRolesQuery query, CancellationToken ct);
}

/// <summary>Returns the system roles (shared, OrganizationId == null) plus any roles scoped
/// specifically to this organization (future custom-role feature).</summary>
public class GetOrganizationRolesHandler : IGetOrganizationRolesHandler
{
    private readonly IApplicationDbContext _db;

    public GetOrganizationRolesHandler(IApplicationDbContext db) => _db = db;

    public async Task<IReadOnlyList<RoleDto>> Handle(GetOrganizationRolesQuery query, CancellationToken ct)
    {
        return await _db.Roles.AsNoTracking()
            .Where(r => r.OrganizationId == null || r.OrganizationId == query.OrganizationId)
            .Select(r => new RoleDto(
                r.Id, r.Name, r.IsSystemRole,
                r.RolePermissions.Select(rp => rp.Permission.Key).ToList()))
            .ToListAsync(ct);
    }
}
