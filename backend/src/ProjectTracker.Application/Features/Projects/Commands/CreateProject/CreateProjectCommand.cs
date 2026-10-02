using ProjectTracker.Domain.Enums;

namespace ProjectTracker.Application.Features.Projects.Commands.CreateProject;

public record CreateProjectCommand(
    Guid OrganizationId, Guid CreatedByUserId, string Key, string Name, string? Description,
    Guid? ManagerUserId, Priority Priority, DateOnly? StartDate, DateOnly? TargetDate);
