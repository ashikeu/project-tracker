namespace ProjectTracker.Application.Features.Sync.Dtos;

public record SyncChangeDto(long ServerVersion, string EntityType, Guid EntityId, string ChangeType, string? PayloadJson);

public record SyncPullResult(IReadOnlyList<SyncChangeDto> Changes, long NextVersion, bool HasMore);

// --- Push ---

public record SyncOperationInput(
    Guid OperationId, string EntityType, Guid EntityId, string ChangeType,
    string? PayloadJson, string? BaseRowVersion);

public record SyncPushRequest(IReadOnlyList<SyncOperationInput> Operations);

public record SyncOperationResult(
    Guid OperationId, string Status, string? Message,
    string? TaskKey, string? RowVersion, string? ServerPayloadJson);

public record SyncPushResponse(IReadOnlyList<SyncOperationResult> Results);
