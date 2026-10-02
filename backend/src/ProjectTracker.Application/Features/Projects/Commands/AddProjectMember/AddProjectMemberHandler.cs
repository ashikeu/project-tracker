using Microsoft.EntityFrameworkCore;
using ProjectTracker.Application.Common.Interfaces;
using ProjectTracker.Application.Features.Projects.Dtos;
using ProjectTracker.Domain.Entities;
using ProjectTracker.Shared.Results;

namespace ProjectTracker.Application.Features.Projects.Commands.AddProjectMember;

public interface IAddProjectMemberHandler
{
    Task<Result<ProjectMemberDto>> Handle(AddProjectMemberCommand command, CancellationToken ct);
}

public class AddProjectMemberHandler : IAddProjectMemberHandler
{
    private readonly IApplicationDbContext _db;

    public AddProjectMemberHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result<ProjectMemberDto>> Handle(AddProjectMemberCommand command, CancellationToken ct)
    {
        var project = await _db.Projects.AsNoTracking().FirstOrDefaultAsync(p => p.Id == command.ProjectId, ct);
        if (project is null) return Result<ProjectMemberDto>.Fail("Project not found.");

        var isOrgMember = await _db.OrganizationUsers.AsNoTracking()
            .AnyAsync(ou => ou.OrganizationId == project.OrganizationId && ou.UserId == command.UserId, ct);
        if (!isOrgMember)
        {
            return Result<ProjectMemberDto>.FailValidation(new[]
            {
                new ValidationError(nameof(command.UserId), "This user is not a member of the organization.")
            });
        }

        var role = await _db.Roles.AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == command.RoleId && (r.OrganizationId == null || r.OrganizationId == project.OrganizationId), ct);
        if (role is null)
        {
            return Result<ProjectMemberDto>.FailValidation(new[]
            {
                new ValidationError(nameof(command.RoleId), "Role not found for this organization.")
            });
        }

        var alreadyMember = await _db.ProjectMembers.AsNoTracking()
            .AnyAsync(pm => pm.ProjectId == command.ProjectId && pm.UserId == command.UserId, ct);
        if (alreadyMember)
        {
            return Result<ProjectMemberDto>.Fail("This user is already a member of the project.");
        }

        var member = new ProjectMember { ProjectId = command.ProjectId, UserId = command.UserId, RoleId = command.RoleId };
        _db.ProjectMembers.Add(member);
        await _db.SaveChangesAsync(ct);

        var user = await _db.Users.AsNoTracking().FirstAsync(u => u.Id == command.UserId, ct);
        return Result<ProjectMemberDto>.Ok(new ProjectMemberDto(member.Id, user.Id, user.Email, user.FirstName, user.LastName, user.AvatarUrl, role.Id, role.Name));
    }
}
