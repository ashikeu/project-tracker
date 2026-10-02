using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ProjectTracker.Application.Common.Interfaces;
using ProjectTracker.Domain.Enums;

namespace ProjectTracker.Infrastructure.Notifications;

/// <summary>Hourly background scan that notifies assignees of tasks due within the next 24 hours.
/// Runs outside any request, so there is no OrganizationId claim — it deliberately bypasses the
/// tenant global query filter (IgnoreQueryFilters) and scans across all organizations, re-applying
/// the not-deleted condition by hand. Deduplicated: a given task+assignee is not re-notified within
/// a 24h window, so the hourly cadence doesn't spam.</summary>
public class DueSoonNotificationJob : BackgroundService
{
    private static readonly TimeSpan Interval = TimeSpan.FromHours(1);
    private static readonly TimeSpan DueWindow = TimeSpan.FromHours(24);
    private static readonly TimeSpan RenotifyWindow = TimeSpan.FromHours(24);

    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<DueSoonNotificationJob> _logger;

    public DueSoonNotificationJob(IServiceScopeFactory scopeFactory, ILogger<DueSoonNotificationJob> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Small initial delay so startup (migrations/seeding) settles before the first scan.
        try { await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken); }
        catch (OperationCanceledException) { return; }

        using var timer = new PeriodicTimer(Interval);
        do
        {
            try
            {
                await RunOnceAsync(stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Due-soon notification scan failed; will retry next interval.");
            }
        }
        while (await timer.WaitForNextTickAsync(stoppingToken));
    }

    private async Task RunOnceAsync(CancellationToken ct)
    {
        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();
        var dispatcher = scope.ServiceProvider.GetRequiredService<INotificationDispatcher>();

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var cutoffDate = DateOnly.FromDateTime(DateTime.UtcNow.Add(DueWindow));
        var renotifyCutoff = DateTime.UtcNow - RenotifyWindow;

        var dueTasks = await db.Tasks
            .IgnoreQueryFilters()
            .Where(t => !t.IsDeleted
                        && t.AssigneeUserId != null
                        && t.DueDate != null
                        && t.DueDate >= today
                        && t.DueDate <= cutoffDate
                        && t.Status != TaskItemStatus.Done)
            .Select(t => new { t.Id, t.TaskKey, t.Title, t.DueDate, AssigneeUserId = t.AssigneeUserId!.Value })
            .ToListAsync(ct);

        var notified = 0;
        foreach (var task in dueTasks)
        {
            var alreadyNotified = await db.Notifications.IgnoreQueryFilters().AnyAsync(n =>
                n.UserId == task.AssigneeUserId
                && n.Type == NotificationType.TaskDueSoon
                && n.RelatedEntityId == task.Id
                && n.CreatedAt > renotifyCutoff, ct);
            if (alreadyNotified) continue;

            await dispatcher.DispatchAsync(
                new[] { task.AssigneeUserId }, NotificationType.TaskDueSoon,
                $"{task.TaskKey} is due {task.DueDate:yyyy-MM-dd}", task.Title, "Task", task.Id, ct);
            notified++;
        }

        if (notified > 0)
        {
            _logger.LogInformation("Due-soon scan sent {Count} notification(s).", notified);
        }
    }
}
