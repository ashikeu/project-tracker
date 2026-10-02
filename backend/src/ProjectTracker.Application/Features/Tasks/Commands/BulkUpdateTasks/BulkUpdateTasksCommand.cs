using ProjectTracker.Domain.Enums;

namespace ProjectTracker.Application.Features.Tasks.Commands.BulkUpdateTasks;

/// <summary>Each "Change*" flag disambiguates "set this field to null/unassigned" from "leave
/// this field untouched" — a plain nullable value alone can't express that distinction.</summary>
public record BulkUpdateTasksCommand(
    IReadOnlyList<Guid> TaskIds,
    bool ChangeAssignee, Guid? AssigneeUserId,
    TaskItemStatus? Status,
    Priority? Priority,
    bool ChangeEpic, Guid? EpicId,
    bool ChangeSprint, Guid? SprintId,
    Guid? AddLabelId);
