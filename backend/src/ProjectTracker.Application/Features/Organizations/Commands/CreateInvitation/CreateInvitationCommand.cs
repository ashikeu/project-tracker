namespace ProjectTracker.Application.Features.Organizations.Commands.CreateInvitation;

public record CreateInvitationCommand(Guid OrganizationId, Guid InvitedByUserId, string Email, Guid RoleId);
