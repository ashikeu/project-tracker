using ProjectTracker.Domain.Enums;

namespace ProjectTracker.Application.Features.Projects.Commands.UpdateProject;

public record UpdateProjectCommand(
    Guid ProjectId, string Name, string? Description, Guid? ManagerUserId,
    ProjectStatus Status, Priority Priority, DateOnly? StartDate, DateOnly? TargetDate);
