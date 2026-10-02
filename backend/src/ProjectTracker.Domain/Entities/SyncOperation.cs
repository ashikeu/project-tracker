using ProjectTracker.Domain.Common;
using ProjectTracker.Domain.Enums;

namespace ProjectTracker.Domain.Entities;

/// <summary>Idempotency ledger for pushed operations. PK Id IS the client-generated OperationId, so
/// re-pushing the same operation (flaky-connection retry) is a duplicate-key hit the push handler
/// short-circuits — replaying the stored ResultJson rather than applying twice. The server is the
/// dedup authority (planning.md §9 risks).</summary>
public class SyncOperation : BaseEntity, ITenantEntity
{
    public Guid OrganizationId { get; set; }
    public Guid UserId { get; set; }

    public string EntityType { get; set; } = string.Empty;
    public Guid EntityId { get; set; }

    public SyncOperationStatus Status { get; set; }

    /// <summary>The serialized per-op result returned on first processing, replayed verbatim on a
    /// duplicate push so the client sees an identical outcome.</summary>
    public string? ResultJson { get; set; }

    public DateTime ProcessedAt { get; set; }
}
