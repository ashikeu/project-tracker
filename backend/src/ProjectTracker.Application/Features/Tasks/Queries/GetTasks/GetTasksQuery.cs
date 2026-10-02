using ProjectTracker.Domain.Enums;

namespace ProjectTracker.Application.Features.Tasks.Queries.GetTasks;

public record GetTasksQuery(
    Guid? ProjectId, Guid? EpicId, Guid? SprintId, TaskItemStatus? Status, Priority? Priority, TaskType? TaskType,
    Guid? AssigneeUserId, Guid? ReporterUserId, Guid? TeamId, Guid? LabelId,
    DateOnly? DueBefore, DateOnly? DueAfter, string? Search, bool UnassignedOnly, bool TopLevelOnly,
    string? SortBy, bool SortDescending, int Page, int PageSize);
