using ProjectTracker.Domain.Common;
using ProjectTracker.Domain.Enums;

namespace ProjectTracker.Domain.Entities;

public class Organization : BaseAuditableEntity
{
    public string Name { get; set; } = string.Empty;

    /// <summary>URL-safe unique identifier, e.g. "acme-corp".</summary>
    public string Slug { get; set; } = string.Empty;

    public OrganizationStatus Status { get; set; } = OrganizationStatus.Active;

    /// <summary>Free-form org settings (branding, defaults) serialized as JSON.</summary>
    public string? SettingsJson { get; set; }

    public ICollection<OrganizationUser> Members { get; set; } = new List<OrganizationUser>();
    public ICollection<Role> Roles { get; set; } = new List<Role>();
}
