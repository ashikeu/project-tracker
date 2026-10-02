using Microsoft.EntityFrameworkCore;
using ProjectTracker.Application.Common.Interfaces;
using ProjectTracker.Application.Features.TimeTracking.Dtos;
using ProjectTracker.Shared.Results;

namespace ProjectTracker.Application.Features.TimeTracking.Queries.GetTaskTimeSummary;

public record GetTaskTimeSummaryQuery(Guid TaskId);

public interface IGetTaskTimeSummaryHandler
{
    Task<Result<TaskTimeSummaryDto>> Handle(GetTaskTimeSummaryQuery query, CancellationToken ct);
}

public class GetTaskTimeSummaryHandler : IGetTaskTimeSummaryHandler
{
    private readonly IApplicationDbContext _db;

    public GetTaskTimeSummaryHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result<TaskTimeSummaryDto>> Handle(GetTaskTimeSummaryQuery query, CancellationToken ct)
    {
        var task = await _db.Tasks.AsNoTracking()
            .Where(t => t.Id == query.TaskId)
            .Select(t => new { t.Id, t.EstimatedHours, t.LoggedHours, t.RemainingHours })
            .FirstOrDefaultAsync(ct);
        if (task is null) return Result<TaskTimeSummaryDto>.Fail("Task not found.");

        return Result<TaskTimeSummaryDto>.Ok(new TaskTimeSummaryDto(
            task.Id, task.EstimatedHours, task.LoggedHours, task.RemainingHours));
    }
}
