using Microsoft.EntityFrameworkCore;
using ProjectTracker.Domain.Entities;

namespace ProjectTracker.Application.Common.Interfaces;

/// <summary>Application-layer view of the persistence context. Infrastructure's
/// ApplicationDbContext implements this so Application handlers depend only on this
/// interface, never on EF Core's DbContext type or Infrastructure directly.</summary>
public interface IApplicationDbContext
{
    DbSet<Organization> Organizations { get; }
    DbSet<User> Users { get; }
    DbSet<OrganizationUser> OrganizationUsers { get; }
    DbSet<Role> Roles { get; }
    DbSet<Permission> Permissions { get; }
    DbSet<RolePermission> RolePermissions { get; }
    DbSet<RefreshToken> RefreshTokens { get; }

    DbSet<Team> Teams { get; }
    DbSet<TeamMember> TeamMembers { get; }
    DbSet<Project> Projects { get; }
    DbSet<ProjectMember> ProjectMembers { get; }
    DbSet<ProjectTeam> ProjectTeams { get; }

    DbSet<Epic> Epics { get; }
    DbSet<Sprint> Sprints { get; }
    DbSet<TaskItem> Tasks { get; }
    DbSet<Label> Labels { get; }
    DbSet<TaskLabel> TaskLabels { get; }
    DbSet<TaskWatcher> TaskWatchers { get; }
    DbSet<ChecklistItem> ChecklistItems { get; }
    DbSet<TaskDependency> TaskDependencies { get; }
    DbSet<BoardColumn> BoardColumns { get; }

    DbSet<Comment> Comments { get; }
    DbSet<CommentMention> CommentMentions { get; }
    DbSet<Attachment> Attachments { get; }
    DbSet<TimeEntry> TimeEntries { get; }

    DbSet<Notification> Notifications { get; }
    DbSet<NotificationPreference> NotificationPreferences { get; }
    DbSet<ActivityLog> ActivityLogs { get; }
    DbSet<SavedFilter> SavedFilters { get; }

    DbSet<SyncChange> SyncChanges { get; }
    DbSet<SyncOperation> SyncOperations { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
