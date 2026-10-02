namespace ProjectTracker.Application.Features.Sprints.Commands.UpdateSprint;

/// <summary>Edits descriptive fields only. Status transitions are deliberately excluded here —
/// they go through the dedicated start/complete/cancel endpoints so the state-machine guard
/// (SprintStatusTransition) is the single gate for status changes.</summary>
public record UpdateSprintCommand(Guid SprintId, string Name, string? Goal, DateOnly? StartDate, DateOnly? EndDate);
