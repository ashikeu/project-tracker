using ProjectTracker.Domain.Common;

namespace ProjectTracker.Domain.Entities;

public class Team : BaseSoftDeleteEntity, ITenantEntity
{
    public Guid OrganizationId { get; set; }
    public Organization Organization { get; set; } = null!;

    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }

    public Guid? LeadUserId { get; set; }
    public User? Lead { get; set; }

    public bool IsArchived { get; set; }

    public ICollection<TeamMember> Members { get; set; } = new List<TeamMember>();
    public ICollection<ProjectTeam> ProjectTeams { get; set; } = new List<ProjectTeam>();
}
