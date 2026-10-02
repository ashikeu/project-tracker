using ProjectTracker.Domain.Common;
using ProjectTracker.Domain.Enums;

namespace ProjectTracker.Domain.Entities;

/// <summary>Membership of a User in an Organization, with the Role that grants their
/// permissions within that organization. A User may belong to multiple Organizations, each with
/// a separate OrganizationUser row and its own Role.</summary>
public class OrganizationUser : BaseAuditableEntity, ITenantEntity
{
    public Guid OrganizationId { get; set; }
    public Organization Organization { get; set; } = null!;

    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public Guid RoleId { get; set; }
    public Role Role { get; set; } = null!;

    public OrganizationUserStatus Status { get; set; } = OrganizationUserStatus.Invited;
    public DateTime? JoinedAt { get; set; }
    public Guid? InvitedByUserId { get; set; }
}
