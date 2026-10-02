using ProjectTracker.Domain.Enums;

namespace ProjectTracker.Application.Features.Teams.Commands.AddTeamMember;

public record AddTeamMemberCommand(Guid TeamId, Guid UserId, TeamMemberRole Role);
