namespace ProjectTracker.Application.Features.Reports.Dtos;

public record BurndownPoint(DateOnly Date, double IdealRemaining, double? ActualRemaining);

public record BurndownDto(
    Guid SprintId, string Name, DateOnly StartDate, DateOnly EndDate,
    int TotalStoryPoints, IReadOnlyList<BurndownPoint> Points);

public record SprintReportDto(
    Guid SprintId, string Name, string Status, DateOnly? StartDate, DateOnly? EndDate, DateTime? CompletedAt,
    int TotalTasks, int CompletedTasks, int CommittedStoryPoints, int CompletedStoryPoints,
    double CompletionPercent, IReadOnlyList<StatusCount> TasksByStatus);

public record StatusCount(string Status, int Count);

public record WorkloadReportRow(
    Guid? UserId, string UserName, int OpenTasks, int OpenStoryPoints, int OverdueTasks, decimal LoggedHours);

public record WorkloadReportDto(Guid ProjectId, IReadOnlyList<WorkloadReportRow> Rows);

public record EpicProgressReportRow(
    Guid EpicId, string Name, string Status, int TotalTasks, int DoneTasks, int TotalStoryPoints, int DoneStoryPoints,
    double CompletionPercent);

public record EpicProgressReportDto(Guid ProjectId, IReadOnlyList<EpicProgressReportRow> Rows);

public record TimeTrackingReportRow(Guid UserId, string UserName, int EntryCount, decimal LoggedHours, decimal BillableHours);

public record TimeTrackingReportDto(
    Guid ProjectId, DateOnly? From, DateOnly? To,
    decimal TotalLoggedHours, decimal TotalBillableHours, IReadOnlyList<TimeTrackingReportRow> Rows);

public record TeamProductivityRow(Guid UserId, string UserName, int CompletedTasks, int CompletedStoryPoints, decimal LoggedHours);

public record TeamProductivityDto(
    Guid ProjectId, DateOnly From, DateOnly To, IReadOnlyList<TeamProductivityRow> Rows);

public record CompletionTrendPoint(DateOnly Date, int Created, int Completed);

public record CompletionReportDto(
    Guid ProjectId, DateOnly From, DateOnly To,
    int TotalCreated, int TotalCompleted, IReadOnlyList<CompletionTrendPoint> Points);
