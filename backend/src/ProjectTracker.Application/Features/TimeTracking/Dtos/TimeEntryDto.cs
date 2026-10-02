namespace ProjectTracker.Application.Features.TimeTracking.Dtos;

public record TimeEntryDto(
    Guid Id, Guid TaskId, Guid UserId, string UserName, DateOnly Date, int DurationMinutes,
    bool IsBillable, string? Description, bool IsRunningTimer, DateTime? StartedAt, DateTime CreatedAt);

/// <summary>Rolled-up time picture for a task: the estimate, hours logged so far (sum of finalized
/// entries), and remaining (estimate − logged, floored at zero; null when there's no estimate).</summary>
public record TaskTimeSummaryDto(
    Guid TaskId, decimal? EstimatedHours, decimal LoggedHours, decimal? RemainingHours);
