namespace ProjectTracker.Application.Features.Sprints.Commands.CreateSprint;

public record CreateSprintCommand(Guid ProjectId, string Name, string? Goal, DateOnly? StartDate, DateOnly? EndDate);
