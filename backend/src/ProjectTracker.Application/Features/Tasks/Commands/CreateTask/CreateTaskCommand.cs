using ProjectTracker.Domain.Enums;

namespace ProjectTracker.Application.Features.Tasks.Commands.CreateTask;

public record CreateTaskCommand(
    Guid ProjectId, Guid ReporterUserId, Guid? EpicId, Guid? ParentTaskId,
    string Title, string? Description, TaskType TaskType, Priority Priority,
    Guid? AssigneeUserId, Guid? TeamId, int? StoryPoints, decimal? EstimatedHours,
    DateOnly? StartDate, DateOnly? DueDate, IReadOnlyList<Guid>? LabelIds);
