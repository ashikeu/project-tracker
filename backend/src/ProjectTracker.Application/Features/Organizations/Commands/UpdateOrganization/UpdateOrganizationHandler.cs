using Microsoft.EntityFrameworkCore;
using ProjectTracker.Application.Common.Interfaces;
using ProjectTracker.Application.Features.Organizations.Dtos;
using ProjectTracker.Shared.Results;

namespace ProjectTracker.Application.Features.Organizations.Commands.UpdateOrganization;

public interface IUpdateOrganizationHandler
{
    Task<Result<OrganizationDto>> Handle(UpdateOrganizationCommand command, CancellationToken ct);
}

public class UpdateOrganizationHandler : IUpdateOrganizationHandler
{
    private readonly IApplicationDbContext _db;

    public UpdateOrganizationHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result<OrganizationDto>> Handle(UpdateOrganizationCommand command, CancellationToken ct)
    {
        var org = await _db.Organizations.FirstOrDefaultAsync(o => o.Id == command.OrganizationId, ct);
        if (org is null)
        {
            return Result<OrganizationDto>.Fail("Organization not found.");
        }

        org.Name = command.Name.Trim();
        await _db.SaveChangesAsync(ct);

        return Result<OrganizationDto>.Ok(new OrganizationDto(org.Id, org.Name, org.Slug, org.Status.ToString(), org.CreatedAt));
    }
}
