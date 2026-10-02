namespace ProjectTracker.Domain.Common;

public abstract class BaseEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();

    private readonly List<BaseDomainEvent> _domainEvents = new();
    public IReadOnlyList<BaseDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    public void AddDomainEvent(BaseDomainEvent domainEvent) => _domainEvents.Add(domainEvent);

    public void ClearDomainEvents() => _domainEvents.Clear();
}

public abstract class BaseAuditableEntity : BaseEntity
{
    public DateTime CreatedAt { get; set; }
    public Guid? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public Guid? UpdatedBy { get; set; }

    /// <summary>SQL Server rowversion — optimistic concurrency token, also drives sync conflict detection.</summary>
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();
}

public abstract class BaseSoftDeleteEntity : BaseAuditableEntity
{
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public Guid? DeletedBy { get; set; }
}

/// <summary>Marker for entities owned by an Organization; used to apply the tenant-isolation
/// global query filter uniformly in ApplicationDbContext.OnModelCreating.</summary>
public interface ITenantEntity
{
    Guid OrganizationId { get; set; }
}

/// <summary>Marker for entities whose changes are captured into the sync change-feed (SyncChanges)
/// so offline clients can pull deltas. The outbox in ApplicationDbContext.SaveChangesAsync emits a
/// SyncChange row for every insert/update/soft-delete of an entity carrying this marker. Must also
/// be a BaseEntity (for Id) and ITenantEntity (for OrganizationId).</summary>
public interface ISyncableEntity
{
}
