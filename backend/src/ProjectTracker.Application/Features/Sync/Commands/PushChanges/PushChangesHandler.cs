using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using ProjectTracker.Application.Common.Interfaces;
using ProjectTracker.Application.Features.Sync.Dtos;
using ProjectTracker.Domain.Entities;
using ProjectTracker.Domain.Enums;
using ProjectTracker.Shared.Results;

namespace ProjectTracker.Application.Features.Sync.Commands.PushChanges;

public interface IPushChangesHandler
{
    Task<Result<SyncPushResponse>> Handle(SyncPushRequest request, CancellationToken ct);
}

/// <summary>Applies a batch of client operations. Each is idempotent by OperationId (a duplicate
/// replays the stored result), and updates use optimistic concurrency: a client whose BaseRowVersion
/// no longer matches the server's gets a Conflict carrying the current server payload — the server
/// never silently overwrites newer data. Applying a task mutation flows through the normal
/// SaveChanges outbox, so it also emits a SyncChange other clients will pull.
/// Scope: TaskItem is the offline-editable entity; other entity types are rejected (the wire format
/// carries EntityType so more types can be added behind the same endpoint).</summary>
public class PushChangesHandler : IPushChangesHandler
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public PushChangesHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<Result<SyncPushResponse>> Handle(SyncPushRequest request, CancellationToken ct)
    {
        if (request.Operations is null || request.Operations.Count == 0)
        {
            return Result<SyncPushResponse>.Fail("No operations to push.");
        }
        if (request.Operations.Count > 200)
        {
            return Result<SyncPushResponse>.Fail("A sync push is limited to 200 operations per batch.");
        }

        var results = new List<SyncOperationResult>();
        foreach (var op in request.Operations)
        {
            results.Add(await ProcessAsync(op, ct));
        }
        return Result<SyncPushResponse>.Ok(new SyncPushResponse(results));
    }

    private async Task<SyncOperationResult> ProcessAsync(SyncOperationInput op, CancellationToken ct)
    {
        // Idempotency: a previously-processed OperationId replays its stored result verbatim.
        var existing = await _db.SyncOperations.AsNoTracking().FirstOrDefaultAsync(o => o.Id == op.OperationId, ct);
        if (existing is not null)
        {
            return existing.ResultJson is not null
                ? JsonSerializer.Deserialize<SyncOperationResult>(existing.ResultJson)!
                : new SyncOperationResult(op.OperationId, existing.Status.ToString(), "Replayed.", null, null, null);
        }

        SyncOperationResult result;
        if (!string.Equals(op.EntityType, "Task", StringComparison.OrdinalIgnoreCase))
        {
            result = Rejected(op, $"Entity type '{op.EntityType}' is not supported for sync push.");
        }
        else
        {
            result = op.ChangeType.ToLowerInvariant() switch
            {
                "created" or "create" => await ApplyCreateAsync(op, ct),
                "updated" or "update" => await ApplyUpdateAsync(op, ct),
                "deleted" or "delete" => await ApplyDeleteAsync(op, ct),
                _ => Rejected(op, $"Unknown change type '{op.ChangeType}'.")
            };
        }

        await RecordOperationAsync(op, result, ct);
        return result;
    }

    private async Task<SyncOperationResult> ApplyCreateAsync(SyncOperationInput op, CancellationToken ct)
    {
        var payload = Deserialize(op.PayloadJson);
        if (payload is null) return Rejected(op, "Missing or invalid task payload.");

        // Idempotent create: if the row already exists (e.g. a prior push landed but the ack was
        // lost), return the current server state rather than failing on a duplicate key.
        var already = await _db.Tasks.AsNoTracking().FirstOrDefaultAsync(t => t.Id == op.EntityId, ct);
        if (already is not null) return Applied(op, already);

        var orgId = _currentUser.OrganizationId!.Value;
        var project = await _db.Projects.FirstOrDefaultAsync(p => p.Id == payload.ProjectId, ct);
        if (project is null) return Rejected(op, "Project not found for offline-created task.");

        var assignedNumber = project.NextTaskNumber;
        project.NextTaskNumber++;

        var task = new TaskItem
        {
            Id = op.EntityId,
            OrganizationId = orgId,
            ProjectId = payload.ProjectId,
            TaskKey = $"{project.Key}-{assignedNumber}",
            EpicId = payload.EpicId,
            SprintId = payload.SprintId,
            ParentTaskId = payload.ParentTaskId,
            Title = payload.Title.Trim(),
            Description = payload.Description?.Trim(),
            TaskType = (TaskType)payload.TaskType,
            Status = (TaskItemStatus)payload.Status,
            Priority = (Priority)payload.Priority,
            AssigneeUserId = payload.AssigneeUserId,
            ReporterUserId = _currentUser.UserId!.Value,
            TeamId = payload.TeamId,
            BoardColumnId = payload.BoardColumnId,
            StoryPoints = payload.StoryPoints,
            EstimatedHours = payload.EstimatedHours,
            RemainingHours = payload.RemainingHours,
            StartDate = payload.StartDate,
            DueDate = payload.DueDate,
            Position = payload.Position,
            ClientGeneratedId = op.EntityId
        };
        _db.Tasks.Add(task);

        try
        {
            await _db.SaveChangesAsync(ct);
        }
        catch (DbUpdateException ex)
        {
            return Rejected(op, $"Could not create task: {RootMessage(ex)}");
        }
        return Applied(op, task);
    }

    private async Task<SyncOperationResult> ApplyUpdateAsync(SyncOperationInput op, CancellationToken ct)
    {
        var payload = Deserialize(op.PayloadJson);
        if (payload is null) return Rejected(op, "Missing or invalid task payload.");

        var task = await _db.Tasks.FirstOrDefaultAsync(t => t.Id == op.EntityId, ct);
        if (task is null) return Rejected(op, "Task not found on the server.");

        var currentRowVersion = Convert.ToBase64String(task.RowVersion ?? Array.Empty<byte>());
        if (!string.IsNullOrEmpty(op.BaseRowVersion) && op.BaseRowVersion != currentRowVersion)
        {
            // Never overwrite newer server data — hand the client both versions to reconcile.
            return Conflict(op, task);
        }

        task.Title = payload.Title.Trim();
        task.Description = payload.Description?.Trim();
        task.EpicId = payload.EpicId;
        task.SprintId = payload.SprintId;
        task.TaskType = (TaskType)payload.TaskType;
        task.Status = (TaskItemStatus)payload.Status;
        task.Priority = (Priority)payload.Priority;
        task.AssigneeUserId = payload.AssigneeUserId;
        task.TeamId = payload.TeamId;
        task.BoardColumnId = payload.BoardColumnId;
        task.StoryPoints = payload.StoryPoints;
        task.EstimatedHours = payload.EstimatedHours;
        task.RemainingHours = payload.RemainingHours;
        task.StartDate = payload.StartDate;
        task.DueDate = payload.DueDate;
        task.Position = payload.Position;

        try
        {
            await _db.SaveChangesAsync(ct);
        }
        catch (DbUpdateException ex)
        {
            return Rejected(op, $"Could not update task: {RootMessage(ex)}");
        }
        return Applied(op, task);
    }

    private async Task<SyncOperationResult> ApplyDeleteAsync(SyncOperationInput op, CancellationToken ct)
    {
        var task = await _db.Tasks.FirstOrDefaultAsync(t => t.Id == op.EntityId, ct);
        if (task is null) return Applied(op, null); // already gone — idempotent

        var currentRowVersion = Convert.ToBase64String(task.RowVersion ?? Array.Empty<byte>());
        if (!string.IsNullOrEmpty(op.BaseRowVersion) && op.BaseRowVersion != currentRowVersion)
        {
            return Conflict(op, task);
        }

        task.IsDeleted = true;
        await _db.SaveChangesAsync(ct);
        return Applied(op, task);
    }

    private async Task RecordOperationAsync(SyncOperationInput op, SyncOperationResult result, CancellationToken ct)
    {
        _db.SyncOperations.Add(new SyncOperation
        {
            Id = op.OperationId,
            OrganizationId = _currentUser.OrganizationId!.Value,
            UserId = _currentUser.UserId!.Value,
            EntityType = op.EntityType,
            EntityId = op.EntityId,
            Status = Enum.Parse<SyncOperationStatus>(result.Status),
            ResultJson = JsonSerializer.Serialize(result),
            ProcessedAt = DateTime.UtcNow
        });
        await _db.SaveChangesAsync(ct);
    }

    private static readonly JsonSerializerOptions PayloadOptions = new() { PropertyNameCaseInsensitive = true };

    private static TaskSyncPayload? Deserialize(string? json)
    {
        if (string.IsNullOrWhiteSpace(json)) return null;
        try { return JsonSerializer.Deserialize<TaskSyncPayload>(json, PayloadOptions); }
        catch (JsonException) { return null; }
    }

    private static SyncOperationResult Applied(SyncOperationInput op, TaskItem? task) => new(
        op.OperationId, SyncOperationStatus.Applied.ToString(), null,
        task?.TaskKey,
        task is null ? null : Convert.ToBase64String(task.RowVersion ?? Array.Empty<byte>()),
        task is null ? null : JsonSerializer.Serialize(TaskSyncPayload.From(task)));

    private static SyncOperationResult Conflict(SyncOperationInput op, TaskItem serverTask) => new(
        op.OperationId, SyncOperationStatus.Conflict.ToString(),
        "The server has a newer version of this task.",
        serverTask.TaskKey,
        Convert.ToBase64String(serverTask.RowVersion ?? Array.Empty<byte>()),
        JsonSerializer.Serialize(TaskSyncPayload.From(serverTask)));

    private static SyncOperationResult Rejected(SyncOperationInput op, string message) =>
        new(op.OperationId, SyncOperationStatus.Rejected.ToString(), message, null, null, null);

    private static string RootMessage(Exception ex)
    {
        var e = ex;
        while (e.InnerException is not null) e = e.InnerException;
        return e.Message;
    }
}
