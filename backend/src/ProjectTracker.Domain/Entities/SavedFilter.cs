using ProjectTracker.Domain.Common;

namespace ProjectTracker.Domain.Entities;

/// <summary>A user's reusable, named filter (e.g. "My high-priority bugs"). The criteria live in
/// FilterJson as a client-defined shape — the server stores and returns it verbatim rather than
/// interpreting it, so new client filter fields never require a schema change. Scoped to the owner
/// (UserId) within their organization.</summary>
public class SavedFilter : BaseAuditableEntity, ITenantEntity
{
    public Guid OrganizationId { get; set; }

    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    /// <summary>Optional narrowing to a single project; null = an org-wide filter.</summary>
    public Guid? ProjectId { get; set; }

    public string Name { get; set; } = string.Empty;

    /// <summary>What the filter applies to, e.g. "tasks", "backlog", "board".</summary>
    public string Scope { get; set; } = "tasks";

    /// <summary>Opaque client-defined criteria JSON; stored and returned as-is.</summary>
    public string FilterJson { get; set; } = "{}";
}
