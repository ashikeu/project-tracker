using System.Linq.Expressions;
using ProjectTracker.Application.Features.Tasks.Dtos;
using ProjectTracker.Domain.Entities;
using ProjectTracker.Domain.Enums;

namespace ProjectTracker.Application.Features.Tasks.Mappings;

public static class TaskMappings
{
    /// <summary>An Expression (not a compiled delegate) so EF Core can translate it into SQL as
    /// part of a larger query — every Tasks query/handler projects through this one definition
    /// instead of duplicating the shape.</summary>
    public static Expression<Func<TaskItem, TaskDto>> ToDto = t => new TaskDto(
        t.Id, t.TaskKey, t.ProjectId, t.EpicId, t.SprintId, t.ParentTaskId,
        t.Title, t.Description, t.TaskType.ToString(), t.Status.ToString(), t.Priority.ToString(),
        t.AssigneeUserId, t.Assignee != null ? t.Assignee.FirstName + " " + t.Assignee.LastName : null,
        t.ReporterUserId, t.Reporter.FirstName + " " + t.Reporter.LastName,
        t.TeamId, t.Team != null ? t.Team.Name : null,
        t.StoryPoints, t.EstimatedHours, t.RemainingHours, t.LoggedHours,
        t.StartDate, t.DueDate, t.Position,
        t.Subtasks.Count, t.Subtasks.Count(s => s.Status == TaskItemStatus.Done),
        t.TaskLabels.Select(tl => tl.Label.Name).ToList(),
        t.CreatedAt, t.UpdatedAt);
}
