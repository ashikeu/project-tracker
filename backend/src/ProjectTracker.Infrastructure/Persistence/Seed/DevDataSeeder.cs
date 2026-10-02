using Microsoft.EntityFrameworkCore;
using ProjectTracker.Application.Common.Interfaces;
using ProjectTracker.Domain.Entities;
using ProjectTracker.Domain.Enums;
using ProjectTracker.Shared.Constants;

namespace ProjectTracker.Infrastructure.Persistence.Seed;

/// <summary>Development-only convenience data: one organization and five users spanning
/// different roles, so the API and (later) the Flutter app have something to point at
/// immediately. Never runs outside the Development environment (see Program.cs). Idempotent.</summary>
public static class DevDataSeeder
{
    private const string DevPassword = "Passw0rd!"; // dev-only fixed password for all seeded users

    public static async Task SeedAsync(ApplicationDbContext db, IPasswordHasher passwordHasher, CancellationToken ct = default)
    {
        if (await db.Organizations.AnyAsync(ct))
        {
            return; // already seeded
        }

        var roles = await db.Roles.Where(r => r.IsSystemRole).ToDictionaryAsync(r => r.Name, ct);

        var org = new Organization { Name = "Acme Corporation", Slug = "acme-corporation", Status = OrganizationStatus.Active };
        db.Organizations.Add(org);

        (string Email, string First, string Last, string RoleName)[] users =
        {
            ("owner@acme.test", "Olivia", "Owner", SystemRoles.OrganizationOwner),
            ("pm@acme.test", "Priya", "Manager", SystemRoles.ProjectManager),
            ("lead@acme.test", "Liam", "Lead", SystemRoles.TeamLead),
            ("dev@acme.test", "Diego", "Developer", SystemRoles.Developer),
            ("tester@acme.test", "Tina", "Tester", SystemRoles.Tester)
        };

        foreach (var (email, first, last, roleName) in users)
        {
            var user = new User
            {
                Email = email,
                PasswordHash = passwordHasher.Hash(DevPassword),
                FirstName = first,
                LastName = last,
                IsActive = true,
                IsEmailVerified = true
            };
            db.Users.Add(user);

            db.OrganizationUsers.Add(new OrganizationUser
            {
                Organization = org,
                User = user,
                RoleId = roles[roleName].Id,
                Status = OrganizationUserStatus.Active,
                JoinedAt = DateTime.UtcNow
            });
        }

        await db.SaveChangesAsync(ct);
    }
}
