namespace ProjectTracker.Application.Features.Projects.Commands.AddProjectMember;

public record AddProjectMemberCommand(Guid ProjectId, Guid UserId, Guid RoleId);
