namespace ProjectTracker.Application.Features.Sprints.Dtos;

/// <summary>Read model for a Sprint. The story-point totals double as the velocity signal:
/// CompletedStoryPoints on a Completed sprint is that sprint's realized velocity (see
/// GetSprint/GetSprints which compute them from the sprint's tasks).</summary>
public record SprintDto(
    Guid Id, Guid ProjectId, string Name, string? Goal, string Status,
    DateOnly? StartDate, DateOnly? EndDate, DateTime? CompletedAt,
    int TaskCount, int CompletedTaskCount,
    int CommittedStoryPoints, int CompletedStoryPoints,
    DateTime CreatedAt);
