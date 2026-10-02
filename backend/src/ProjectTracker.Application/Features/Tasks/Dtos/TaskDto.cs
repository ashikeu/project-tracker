namespace ProjectTracker.Application.Features.Tasks.Dtos;

public record TaskDto(
    Guid Id, string TaskKey, Guid ProjectId, Guid? EpicId, Guid? SprintId, Guid? ParentTaskId,
    string Title, string? Description, string TaskType, string Status, string Priority,
    Guid? AssigneeUserId, string? AssigneeName, Guid ReporterUserId, string ReporterName,
    Guid? TeamId, string? TeamName,
    int? StoryPoints, decimal? EstimatedHours, decimal? RemainingHours, decimal LoggedHours,
    DateOnly? StartDate, DateOnly? DueDate, double Position,
    int SubtaskCount, int CompletedSubtaskCount,
    IReadOnlyList<string> Labels,
    DateTime CreatedAt, DateTime? UpdatedAt);

public record ChecklistItemDto(Guid Id, string Text, bool IsDone, int Order);

public record TaskDependencyDto(Guid Id, Guid RelatedTaskId, string RelatedTaskKey, string RelatedTaskTitle, string Type);

public record TaskWatcherDto(Guid UserId, string Email, string FirstName, string LastName);

public record TaskDetailDto(
    TaskDto Task, IReadOnlyList<ChecklistItemDto> Checklist,
    IReadOnlyList<TaskDependencyDto> Dependencies, IReadOnlyList<TaskWatcherDto> Watchers);

public record LabelDto(Guid Id, string Name, string Color);
