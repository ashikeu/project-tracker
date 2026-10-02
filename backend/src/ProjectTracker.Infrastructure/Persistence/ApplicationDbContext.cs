using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using ProjectTracker.Application.Common.Interfaces;
using ProjectTracker.Domain.Common;
using ProjectTracker.Domain.Entities;
using ProjectTracker.Domain.Enums;

namespace ProjectTracker.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    private readonly ICurrentUserService? _currentUserService;

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, ICurrentUserService? currentUserService = null)
        : base(options)
    {
        _currentUserService = currentUserService;
    }

    public DbSet<Organization> Organizations => Set<Organization>();
    public DbSet<User> Users => Set<User>();
    public DbSet<OrganizationUser> OrganizationUsers => Set<OrganizationUser>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    public DbSet<Team> Teams => Set<Team>();
    public DbSet<TeamMember> TeamMembers => Set<TeamMember>();
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<ProjectMember> ProjectMembers => Set<ProjectMember>();
    public DbSet<ProjectTeam> ProjectTeams => Set<ProjectTeam>();

    public DbSet<Epic> Epics => Set<Epic>();
    public DbSet<Sprint> Sprints => Set<Sprint>();
    public DbSet<TaskItem> Tasks => Set<TaskItem>();
    public DbSet<Label> Labels => Set<Label>();
    public DbSet<TaskLabel> TaskLabels => Set<TaskLabel>();
    public DbSet<TaskWatcher> TaskWatchers => Set<TaskWatcher>();
    public DbSet<ChecklistItem> ChecklistItems => Set<ChecklistItem>();
    public DbSet<TaskDependency> TaskDependencies => Set<TaskDependency>();
    public DbSet<BoardColumn> BoardColumns => Set<BoardColumn>();

    public DbSet<Comment> Comments => Set<Comment>();
    public DbSet<CommentMention> CommentMentions => Set<CommentMention>();
    public DbSet<Attachment> Attachments => Set<Attachment>();
    public DbSet<TimeEntry> TimeEntries => Set<TimeEntry>();

    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<NotificationPreference> NotificationPreferences => Set<NotificationPreference>();
    public DbSet<ActivityLog> ActivityLogs => Set<ActivityLog>();
    public DbSet<SavedFilter> SavedFilters => Set<SavedFilter>();

    public DbSet<SyncChange> SyncChanges => Set<SyncChange>();
    public DbSet<SyncOperation> SyncOperations => Set<SyncOperation>();

    /// <summary>Read by the global query filter (see BuildQueryFilter) via a `this.` property
    /// access rather than a baked-in constant, so it re-evaluates against whichever
    /// ApplicationDbContext instance is actually executing the query — EF Core caches the
    /// compiled model/filter expression tree once per DbContext type, so anything here must be
    /// an instance member access, never an Expression.Constant captured at model-build time
    /// (that would freeze the very first value seen, e.g. null during startup migration/seeding,
    /// for the lifetime of the process).</summary>
    public Guid? CurrentOrganizationId => _currentUserService?.OrganizationId;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // BaseEntity.DomainEvents is an in-memory-only bag for domain-event publication; it must
        // never be mapped as a persisted collection/entity.
        modelBuilder.Ignore<BaseDomainEvent>();

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        // Combined tenant-isolation + soft-delete global query filter. Every ITenantEntity is
        // scoped to the caller's OrganizationId claim (a missing claim yields zero rows — safe
        // by default rather than leaking data), and every BaseSoftDeleteEntity additionally
        // excludes IsDeleted rows. Built via expression trees so a single filter can apply both
        // conditions when an entity is both (the common case).
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            var clrType = entityType.ClrType;
            var isTenant = typeof(ITenantEntity).IsAssignableFrom(clrType);
            var isSoftDelete = typeof(BaseSoftDeleteEntity).IsAssignableFrom(clrType);
            if (!isTenant && !isSoftDelete) continue;

            var method = typeof(ApplicationDbContext)
                .GetMethod(nameof(BuildQueryFilter), BindingFlags.NonPublic | BindingFlags.Instance)!
                .MakeGenericMethod(clrType);
            var filter = (LambdaExpression)method.Invoke(this, new object[] { isTenant, isSoftDelete })!;
            entityType.SetQueryFilter(filter);
        }

        base.OnModelCreating(modelBuilder);
    }

    private LambdaExpression BuildQueryFilter<TEntity>(bool isTenant, bool isSoftDelete) where TEntity : class
    {
        var parameter = Expression.Parameter(typeof(TEntity), "e");
        Expression? body = null;

        if (isTenant)
        {
            var orgProperty = Expression.Property(parameter, nameof(ITenantEntity.OrganizationId));
            var thisContext = Expression.Constant(this);
            var currentOrg = Expression.Property(thisContext, nameof(CurrentOrganizationId));
            body = Expression.Equal(Expression.Convert(orgProperty, typeof(Guid?)), currentOrg);
        }

        if (isSoftDelete)
        {
            var isDeletedProperty = Expression.Property(parameter, nameof(BaseSoftDeleteEntity.IsDeleted));
            var notDeleted = Expression.Not(isDeletedProperty);
            body = body is null ? notDeleted : Expression.AndAlso(body, notDeleted);
        }

        return Expression.Lambda(body!, parameter);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        var userId = _currentUserService?.UserId;

        foreach (var entry in ChangeTracker.Entries<BaseAuditableEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = now;
                    entry.Entity.CreatedBy = userId;
                    break;
                case EntityState.Modified:
                    entry.Entity.UpdatedAt = now;
                    entry.Entity.UpdatedBy = userId;
                    break;
            }
        }

        // A handler sets IsDeleted = true to soft-delete; stamp DeletedAt/DeletedBy here so
        // every soft-delete path gets consistent audit fields without each handler repeating it.
        foreach (var entry in ChangeTracker.Entries<BaseSoftDeleteEntity>())
        {
            if (entry.State == EntityState.Modified && entry.Entity.IsDeleted && entry.OriginalValues.GetValue<bool>(nameof(BaseSoftDeleteEntity.IsDeleted)) == false)
            {
                entry.Entity.DeletedAt = now;
                entry.Entity.DeletedBy = userId;
            }
        }

        // Sync outbox: capture the change intent for every ISyncableEntity BEFORE saving (so we can
        // still read the pre-save EntityState and the IsDeleted transition). If none, the save is a
        // plain single-transaction save with no extra overhead.
        var syncEntries = CollectSyncEntries();
        if (syncEntries.Count == 0)
        {
            return await base.SaveChangesAsync(cancellationToken);
        }

        // Persist the mutation and its change-feed rows atomically. Change rows are written AFTER
        // the mutation saves so they capture DB-generated values (RowVersion, and TaskKey for
        // offline-created rows) and get their monotonic ServerVersion identity.
        await using var transaction = await Database.BeginTransactionAsync(cancellationToken);
        var result = await base.SaveChangesAsync(cancellationToken);

        foreach (var (entity, changeType) in syncEntries)
        {
            var tenant = (ITenantEntity)entity;
            SyncChanges.Add(new SyncChange
            {
                OrganizationId = tenant.OrganizationId,
                EntityType = SyncEntityTypeName(entity),
                EntityId = ((BaseEntity)entity).Id,
                ChangeType = changeType,
                PayloadJson = changeType == SyncChangeType.Deleted ? null : BuildSyncPayload(entity),
                CreatedAt = now
            });
        }
        await base.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return result;
    }

    private List<(object Entity, SyncChangeType ChangeType)> CollectSyncEntries()
    {
        var entries = new List<(object, SyncChangeType)>();
        foreach (var entry in ChangeTracker.Entries())
        {
            if (entry.Entity is not ISyncableEntity) continue;
            switch (entry.State)
            {
                case EntityState.Added:
                    entries.Add((entry.Entity, SyncChangeType.Created));
                    break;
                case EntityState.Deleted:
                    entries.Add((entry.Entity, SyncChangeType.Deleted));
                    break;
                case EntityState.Modified:
                    var softDeleted = entry.Entity is BaseSoftDeleteEntity
                        && entry.CurrentValues.GetValue<bool>(nameof(BaseSoftDeleteEntity.IsDeleted))
                        && entry.OriginalValues.GetValue<bool>(nameof(BaseSoftDeleteEntity.IsDeleted)) == false;
                    entries.Add((entry.Entity, softDeleted ? SyncChangeType.Deleted : SyncChangeType.Updated));
                    break;
            }
        }
        return entries;
    }

    private static string SyncEntityTypeName(object entity) => entity switch
    {
        TaskItem => "Task",
        _ => entity.GetType().Name
    };

    private static string? BuildSyncPayload(object entity) => entity switch
    {
        TaskItem task => System.Text.Json.JsonSerializer.Serialize(
            Application.Features.Sync.Dtos.TaskSyncPayload.From(task)),
        _ => null
    };
}
