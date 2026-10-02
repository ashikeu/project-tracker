using ProjectTracker.Domain.Common;

namespace ProjectTracker.Domain.Entities;

public class Role : BaseAuditableEntity
{
    /// <summary>Null for the seeded system roles (shared across all organizations); set for a
    /// future custom-role feature scoped to a single organization.</summary>
    public Guid? OrganizationId { get; set; }

    public string Name { get; set; } = string.Empty;
    public bool IsSystemRole { get; set; }

    public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}

public class Permission : BaseEntity
{
    /// <summary>Dot-delimited permission key, e.g. "task.assign" — see PermissionKeys.All.</summary>
    public string Key { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Category { get; set; } = string.Empty;

    public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}

public class RolePermission
{
    public Guid RoleId { get; set; }
    public Role Role { get; set; } = null!;

    public Guid PermissionId { get; set; }
    public Permission Permission { get; set; } = null!;
}
