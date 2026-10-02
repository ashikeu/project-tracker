using Microsoft.EntityFrameworkCore;
using ProjectTracker.Application.Common.Interfaces;
using ProjectTracker.Domain.Entities;
using ProjectTracker.Domain.Enums;
using ProjectTracker.Shared.Results;

namespace ProjectTracker.Application.Features.Organizations.Commands.CreateInvitation;

public interface ICreateInvitationHandler
{
    Task<Result> Handle(CreateInvitationCommand command, CancellationToken ct);
}

/// <summary>Invites a user (by email) into an organization with a given role. If no account
/// exists yet for that email, a placeholder (inactive, no password) User is created; the
/// invitee sets their password via the same reset-password flow when they first sign in. Full
/// invitation-token accept flow is tracked as a Phase 3+ enhancement in tasks.md.</summary>
public class CreateInvitationHandler : ICreateInvitationHandler
{
    private readonly IApplicationDbContext _db;
    private readonly IEmailService _emailService;

    public CreateInvitationHandler(IApplicationDbContext db, IEmailService emailService)
    {
        _db = db;
        _emailService = emailService;
    }

    public async Task<Result> Handle(CreateInvitationCommand command, CancellationToken ct)
    {
        var normalizedEmail = command.Email.Trim().ToLowerInvariant();

        var role = await _db.Roles.AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == command.RoleId
                && (r.OrganizationId == null || r.OrganizationId == command.OrganizationId), ct);
        if (role is null)
        {
            return Result.Fail("Role not found for this organization.");
        }

        var inviter = await _db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == command.InvitedByUserId, ct);
        if (inviter is null)
        {
            return Result.Fail("Inviting user not found.");
        }

        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == normalizedEmail, ct);
        if (user is null)
        {
            user = new User
            {
                Email = normalizedEmail,
                PasswordHash = string.Empty,
                FirstName = string.Empty,
                LastName = string.Empty,
                IsActive = false,
                IsEmailVerified = false
            };
            _db.Users.Add(user);
        }
        else
        {
            var alreadyMember = await _db.OrganizationUsers.AsNoTracking()
                .AnyAsync(ou => ou.OrganizationId == command.OrganizationId && ou.UserId == user.Id, ct);
            if (alreadyMember)
            {
                return Result.Fail("This user is already a member of the organization.");
            }
        }

        var membership = new OrganizationUser
        {
            OrganizationId = command.OrganizationId,
            UserId = user.Id,
            User = user,
            RoleId = role.Id,
            Status = OrganizationUserStatus.Invited,
            InvitedByUserId = command.InvitedByUserId
        };
        _db.OrganizationUsers.Add(membership);

        var organization = await _db.Organizations.AsNoTracking()
            .FirstAsync(o => o.Id == command.OrganizationId, ct);

        await _db.SaveChangesAsync(ct);

        await _emailService.SendOrganizationInvitationAsync(normalizedEmail, organization.Name, inviter.FullName, ct);

        return Result.Ok("Invitation sent.");
    }
}
