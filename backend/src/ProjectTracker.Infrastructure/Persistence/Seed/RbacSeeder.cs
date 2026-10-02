using Microsoft.EntityFrameworkCore;
using ProjectTracker.Domain.Entities;
using ProjectTracker.Shared.Constants;

namespace ProjectTracker.Infrastructure.Persistence.Seed;

/// <summary>Seeds the fixed catalog of Permissions and system Roles (with their
/// role→permission mappings) that the application requires to function — this must run on
/// every environment, not just Development. Idempotent: safe to run on every startup.</summary>
public static class RbacSeeder
{
    private static readonly IReadOnlyDictionary<string, string[]> RoleToPermissions = new Dictionary<string, string[]>
    {
        [SystemRoles.SuperAdmin] = PermissionKeys.All.ToArray(),
        [SystemRoles.OrganizationOwner] = PermissionKeys.All.ToArray(),
        [SystemRoles.OrganizationAdmin] = PermissionKeys.All
            .Where(p => p is not (PermissionKeys.OrganizationManage)).ToArray(),
        [SystemRoles.ProjectManager] = new[]
        {
            PermissionKeys.ProjectCreate, PermissionKeys.ProjectUpdate, PermissionKeys.ProjectDelete,
            PermissionKeys.ProjectView, PermissionKeys.ProjectManageMembers,
            PermissionKeys.EpicCreate, PermissionKeys.EpicUpdate, PermissionKeys.EpicDelete,
            PermissionKeys.TaskView, PermissionKeys.TaskCreate, PermissionKeys.TaskUpdate,
            PermissionKeys.TaskDelete, PermissionKeys.TaskAssign, PermissionKeys.TaskMove,
            PermissionKeys.SprintCreate, PermissionKeys.SprintStart, PermissionKeys.SprintComplete,
            PermissionKeys.CommentCreate, PermissionKeys.CommentDelete,
            PermissionKeys.AttachmentUpload, PermissionKeys.AttachmentDelete,
            PermissionKeys.TimeLog, PermissionKeys.TimeEdit,
            PermissionKeys.TeamView
        },
        [SystemRoles.TeamLead] = new[]
        {
            PermissionKeys.ProjectView, PermissionKeys.TaskView, PermissionKeys.TaskCreate,
            PermissionKeys.TaskUpdate, PermissionKeys.TaskAssign, PermissionKeys.TaskMove,
            PermissionKeys.CommentCreate, PermissionKeys.CommentDelete,
            PermissionKeys.AttachmentUpload, PermissionKeys.AttachmentDelete,
            PermissionKeys.TimeLog, PermissionKeys.TimeEdit, PermissionKeys.TeamView,
            PermissionKeys.TeamManageMembers
        },
        [SystemRoles.Developer] = new[]
        {
            PermissionKeys.ProjectView, PermissionKeys.TaskView, PermissionKeys.TaskCreate,
            PermissionKeys.TaskUpdate, PermissionKeys.TaskAssign, PermissionKeys.TaskMove,
            PermissionKeys.CommentCreate, PermissionKeys.AttachmentUpload,
            PermissionKeys.TimeLog, PermissionKeys.TimeEdit, PermissionKeys.TeamView
        },
        [SystemRoles.Tester] = new[]
        {
            PermissionKeys.ProjectView, PermissionKeys.TaskView, PermissionKeys.TaskCreate,
            PermissionKeys.TaskUpdate, PermissionKeys.TaskAssign, PermissionKeys.TaskMove,
            PermissionKeys.CommentCreate, PermissionKeys.AttachmentUpload,
            PermissionKeys.TimeLog, PermissionKeys.TimeEdit, PermissionKeys.TeamView
        },
        [SystemRoles.Member] = new[]
        {
            PermissionKeys.ProjectView, PermissionKeys.TaskView, PermissionKeys.TaskCreate,
            PermissionKeys.CommentCreate, PermissionKeys.AttachmentUpload, PermissionKeys.TimeLog,
            PermissionKeys.TeamView
        },
        [SystemRoles.Viewer] = new[]
        {
            PermissionKeys.ProjectView, PermissionKeys.TaskView, PermissionKeys.TeamView
        }
    };

    public static async Task SeedAsync(ApplicationDbContext db, CancellationToken ct = default)
    {
        var existingPermissions = await db.Permissions.ToDictionaryAsync(p => p.Key, ct);
        foreach (var key in PermissionKeys.All)
        {
            if (!existingPermissions.ContainsKey(key))
            {
                var category = key.Split('.')[0];
                var permission = new Permission { Key = key, Category = category, Description = key };
                db.Permissions.Add(permission);
                existingPermissions[key] = permission;
            }
        }
        await db.SaveChangesAsync(ct);

        var existingRoles = await db.Roles
            .Where(r => r.IsSystemRole)
            .Include(r => r.RolePermissions)
            .ToDictionaryAsync(r => r.Name, ct);

        foreach (var roleName in SystemRoles.All)
        {
            if (!existingRoles.TryGetValue(roleName, out var role))
            {
                role = new Role { Name = roleName, IsSystemRole = true, OrganizationId = null };
                db.Roles.Add(role);
                existingRoles[roleName] = role;
            }

            var desiredPermissionKeys = RoleToPermissions.TryGetValue(roleName, out var perms)
                ? perms
                : Array.Empty<string>();

            var currentPermissionIds = role.RolePermissions.Select(rp => rp.PermissionId).ToHashSet();
            foreach (var permKey in desiredPermissionKeys)
            {
                var permission = existingPermissions[permKey];
                if (!currentPermissionIds.Contains(permission.Id))
                {
                    role.RolePermissions.Add(new RolePermission { RoleId = role.Id, PermissionId = permission.Id, Role = role, Permission = permission });
                }
            }
        }

        await db.SaveChangesAsync(ct);
    }
}
