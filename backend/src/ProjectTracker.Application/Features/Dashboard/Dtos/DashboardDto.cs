namespace ProjectTracker.Application.Features.Dashboard.Dtos;

public record CountBucket(string Key, int Count);

public record WorkloadRow(Guid? UserId, string UserName, int OpenTaskCount, int StoryPoints);

public record EpicProgressRow(Guid EpicId, string Name, int TotalTasks, int DoneTasks);

public record ActiveSprintSummary(
    Guid Id, string Name, int TaskCount, int DoneTaskCount, int CommittedStoryPoints, int CompletedStoryPoints);

public record DashboardDto(
    Guid ProjectId, string ProjectName,
    int TotalTasks, int OpenTasks, int DoneTasks, int OverdueTasks,
    IReadOnlyList<CountBucket> ByStatus,
    IReadOnlyList<CountBucket> ByPriority,
    IReadOnlyList<CountBucket> ByType,
    IReadOnlyList<WorkloadRow> Workload,
    IReadOnlyList<EpicProgressRow> EpicProgress,
    ActiveSprintSummary? ActiveSprint);
