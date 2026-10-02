namespace ProjectTracker.Domain.Enums;

public enum SyncChangeType
{
    Created = 0,
    Updated = 1,
    Deleted = 2
}

/// <summary>Outcome of applying one client-pushed operation. Stored on the SyncOperation row so a
/// retried push (same OperationId) replays the identical result instead of re-applying.</summary>
public enum SyncOperationStatus
{
    Applied = 0,
    Conflict = 1,
    Rejected = 2
}
