using ProjectTracker.Domain.Enums;

namespace ProjectTracker.Application.Features.Tasks.Commands.UpdateTask;

public record UpdateTaskCommand(
    Guid TaskId, string Title, string? Description, Guid? EpicId, TaskType TaskType,
    TaskItemStatus Status, Priority Priority, Guid? TeamId, int? StoryPoints,
    decimal? EstimatedHours, decimal? RemainingHours, DateOnly? StartDate, DateOnly? DueDate);
