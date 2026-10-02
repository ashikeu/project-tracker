using ProjectTracker.Domain.Entities;

namespace ProjectTracker.Application.Features.Sync.Dtos;

/// <summary>Wire representation of a TaskItem for the sync feed and push results. RowVersion is
/// base64 so the client can round-trip the concurrency token as a string and send it back as the
/// baseRowVersion on a later update.</summary>
public record TaskSyncPayload(
    Guid Id, string TaskKey, Guid ProjectId, Guid? EpicId, Guid? SprintId, Guid? ParentTaskId,
    string Title, string? Description, int TaskType, int Status, int Priority,
    Guid? AssigneeUserId, Guid ReporterUserId, Guid? TeamId, Guid? BoardColumnId,
    int? StoryPoints, decimal? EstimatedHours, decimal? RemainingHours, decimal LoggedHours,
    DateOnly? StartDate, DateOnly? DueDate, double Position,
    bool IsDeleted, string RowVersion, DateTime? UpdatedAt)
{
    public static TaskSyncPayload From(TaskItem t) => new(
        t.Id, t.TaskKey, t.ProjectId, t.EpicId, t.SprintId, t.ParentTaskId,
        t.Title, t.Description, (int)t.TaskType, (int)t.Status, (int)t.Priority,
        t.AssigneeUserId, t.ReporterUserId, t.TeamId, t.BoardColumnId,
        t.StoryPoints, t.EstimatedHours, t.RemainingHours, t.LoggedHours,
        t.StartDate, t.DueDate, t.Position,
        t.IsDeleted, Convert.ToBase64String(t.RowVersion ?? Array.Empty<byte>()), t.UpdatedAt);
}
