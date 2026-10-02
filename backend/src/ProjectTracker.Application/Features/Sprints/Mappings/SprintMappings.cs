using System.Linq.Expressions;
using ProjectTracker.Application.Features.Sprints.Dtos;
using ProjectTracker.Domain.Entities;
using ProjectTracker.Domain.Enums;

namespace ProjectTracker.Application.Features.Sprints.Mappings;

public static class SprintMappings
{
    /// <summary>Single EF-translatable projection reused by every Sprint query so the story-point
    /// and task-count aggregates are defined once.</summary>
    public static Expression<Func<Sprint, SprintDto>> ToDto = s => new SprintDto(
        s.Id, s.ProjectId, s.Name, s.Goal, s.Status.ToString(),
        s.StartDate, s.EndDate, s.CompletedAt,
        s.Tasks.Count,
        s.Tasks.Count(t => t.Status == TaskItemStatus.Done),
        s.Tasks.Sum(t => t.StoryPoints ?? 0),
        s.Tasks.Where(t => t.Status == TaskItemStatus.Done).Sum(t => t.StoryPoints ?? 0),
        s.CreatedAt);
}
