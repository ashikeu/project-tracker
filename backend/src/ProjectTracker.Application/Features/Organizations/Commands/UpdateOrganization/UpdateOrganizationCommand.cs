namespace ProjectTracker.Application.Features.Organizations.Commands.UpdateOrganization;

public record UpdateOrganizationCommand(Guid OrganizationId, string Name);
