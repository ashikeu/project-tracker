using ProjectTracker.Domain.Common;
using ProjectTracker.Domain.Enums;

namespace ProjectTracker.Domain.Entities;

/// <summary>One row in the per-organization change-feed. The client pulls rows with
/// ServerVersion greater than its last-seen watermark to replay server-side changes into its local
/// Drift DB. ServerVersion is a database IDENTITY (globally monotonic); the client only ever reads
/// its own org's rows, so a global sequence still gives it a correct, gap-tolerant watermark.
/// Deletes are tombstones (PayloadJson null).</summary>
public class SyncChange : BaseEntity, ITenantEntity
{
    public Guid OrganizationId { get; set; }

    public string EntityType { get; set; } = string.Empty;
    public Guid EntityId { get; set; }
    public SyncChangeType ChangeType { get; set; }

    /// <summary>Database-generated, globally monotonic. The client's delta cursor.</summary>
    public long ServerVersion { get; set; }

    /// <summary>Serialized entity snapshot for Created/Updated; null for Deleted (tombstone).</summary>
    public string? PayloadJson { get; set; }

    public DateTime CreatedAt { get; set; }
}
