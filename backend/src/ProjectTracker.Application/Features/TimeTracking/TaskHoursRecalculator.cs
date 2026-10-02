using Microsoft.EntityFrameworkCore;
using ProjectTracker.Application.Common.Interfaces;
using ProjectTracker.Domain.Entities;

namespace ProjectTracker.Application.Features.TimeTracking;

/// <summary>Recomputes a task's LoggedHours (and, when an estimate exists, RemainingHours) from the
/// sum of its finalized time entries. Called by every time-entry mutation so the task's rollups are
/// derived, never incrementally patched — which keeps them from drifting out of sync with the
/// entries. The caller is responsible for SaveChanges (the task is tracked and mutated in place).</summary>
public static class TaskHoursRecalculator
{
    public static async Task ApplyAsync(IApplicationDbContext db, TaskItem task, CancellationToken ct)
    {
        var totalMinutes = await db.TimeEntries.AsNoTracking()
            .Where(e => e.TaskId == task.Id && !e.IsRunningTimer)
            .SumAsync(e => (int?)e.DurationMinutes, ct) ?? 0;

        var loggedHours = Math.Round(totalMinutes / 60m, 2);
        task.LoggedHours = loggedHours;
        if (task.EstimatedHours is not null)
        {
            task.RemainingHours = Math.Max(0m, task.EstimatedHours.Value - loggedHours);
        }
    }
}
