using Microsoft.EntityFrameworkCore;
using ProjectTracker.Application.Common.Interfaces;
using ProjectTracker.Application.Features.Organizations.Dtos;
using ProjectTracker.Shared.Results;

namespace ProjectTracker.Application.Features.Organizations.Queries.GetOrganization;

public record GetOrganizationQuery(Guid OrganizationId);

public interface IGetOrganizationHandler
{
    Task<Result<OrganizationDto>> Handle(GetOrganizationQuery query, CancellationToken ct);
}

public class GetOrganizationHandler : IGetOrganizationHandler
{
    private readonly IApplicationDbContext _db;

    public GetOrganizationHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result<OrganizationDto>> Handle(GetOrganizationQuery query, CancellationToken ct)
    {
        var org = await _db.Organizations.AsNoTracking()
            .Where(o => o.Id == query.OrganizationId)
            .Select(o => new OrganizationDto(o.Id, o.Name, o.Slug, o.Status.ToString(), o.CreatedAt))
            .FirstOrDefaultAsync(ct);

        return org is null
            ? Result<OrganizationDto>.Fail("Organization not found.")
            : Result<OrganizationDto>.Ok(org);
    }
}
