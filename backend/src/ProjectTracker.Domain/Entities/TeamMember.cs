using ProjectTracker.Domain.Common;
using ProjectTracker.Domain.Enums;

namespace ProjectTracker.Domain.Entities;

public class TeamMember : BaseAuditableEntity
{
    public Guid TeamId { get; set; }
    public Team Team { get; set; } = null!;

    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public TeamMemberRole Role { get; set; } = TeamMemberRole.Member;
}
