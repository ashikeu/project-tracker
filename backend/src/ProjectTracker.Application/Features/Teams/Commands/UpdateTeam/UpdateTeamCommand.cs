namespace ProjectTracker.Application.Features.Teams.Commands.UpdateTeam;

public record UpdateTeamCommand(Guid TeamId, string Name, string? Description, Guid? LeadUserId);
