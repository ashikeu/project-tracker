namespace ProjectTracker.Application.Features.Teams.Commands.CreateTeam;

public record CreateTeamCommand(Guid OrganizationId, Guid CreatedByUserId, string Name, string? Description, Guid? LeadUserId);
