using Microsoft.EntityFrameworkCore;
using ProjectTracker.Application.Common.Interfaces;
using ProjectTracker.Application.Features.Auth.Dtos;
using ProjectTracker.Domain.Entities;
using ProjectTracker.Domain.Enums;
using ProjectTracker.Shared.Results;

namespace ProjectTracker.Application.Features.Auth.Commands.Login;

public interface ILoginHandler
{
    Task<Result<AuthResultDto>> Handle(LoginCommand command, CancellationToken ct);
}

public class LoginHandler : ILoginHandler
{
    private readonly IApplicationDbContext _db;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenHasher _tokenHasher;
    private readonly IJwtTokenService _tokenService;

    public LoginHandler(IApplicationDbContext db, IPasswordHasher passwordHasher, ITokenHasher tokenHasher, IJwtTokenService tokenService)
    {
        _db = db;
        _passwordHasher = passwordHasher;
        _tokenHasher = tokenHasher;
        _tokenService = tokenService;
    }

    public async Task<Result<AuthResultDto>> Handle(LoginCommand command, CancellationToken ct)
    {
        var normalizedEmail = command.Email.Trim().ToLowerInvariant();
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == normalizedEmail, ct);

        // Constant error message regardless of which check failed, to avoid user enumeration.
        const string invalidCredentials = "Invalid email or password.";

        if (user is null || !_passwordHasher.Verify(command.Password, user.PasswordHash))
        {
            return Result<AuthResultDto>.Fail(invalidCredentials);
        }

        if (!user.IsActive)
        {
            return Result<AuthResultDto>.Fail("This account has been deactivated.");
        }

        // IgnoreQueryFilters: the tenant-isolation filter scopes queries to the CALLER's current
        // OrganizationId claim, which doesn't exist yet at login time — this is the one place
        // we intentionally look across all of a specific, already-credential-verified user's
        // memberships to find which organization(s) to issue a token for.
        var memberships = await _db.OrganizationUsers
            .IgnoreQueryFilters()
            .Where(ou => ou.UserId == user.Id && ou.Status == OrganizationUserStatus.Active)
            .Include(ou => ou.Organization)
            .Include(ou => ou.Role)
            .ToListAsync(ct);

        if (memberships.Count == 0)
        {
            return Result<AuthResultDto>.Fail("This account is not a member of any active organization.");
        }

        var selected = command.OrganizationId is null
            ? (memberships.Count == 1 ? memberships[0] : null)
            : memberships.FirstOrDefault(m => m.OrganizationId == command.OrganizationId);

        if (selected is null)
        {
            return Result<AuthResultDto>.Fail(memberships.Count > 1
                ? "Multiple organizations found for this account; specify organizationId."
                : "You are not a member of the specified organization.");
        }

        var permissions = await _db.RolePermissions.AsNoTracking()
            .Where(rp => rp.RoleId == selected.RoleId)
            .Select(rp => rp.Permission.Key)
            .ToListAsync(ct);

        var accessToken = _tokenService.IssueAccessToken(user, selected.Organization, selected.Role, permissions);
        var refreshTokenValue = _tokenService.GenerateRefreshTokenValue();
        var refreshToken = new RefreshToken
        {
            User = user,
            TokenHash = _tokenHasher.Hash(refreshTokenValue),
            ExpiresAt = DateTime.UtcNow.AddDays(30),
            CreatedByIp = command.IpAddress
        };
        _db.RefreshTokens.Add(refreshToken);

        // Persist the refresh token first, on its own — an INSERT has no concurrency token, so
        // concurrent logins never collide here.
        await _db.SaveChangesAsync(ct);

        // LastLoginAt is a best-effort "last writer wins" timestamp, saved separately. Writing it on
        // the tracked, rowversioned User makes two concurrent logins for the same account collide on
        // the optimistic-concurrency token: the first commit bumps RowVersion and the rest throw
        // DbUpdateConcurrencyException — which previously surfaced as a 500 even though authentication
        // fully succeeded. Since the timestamp is non-critical, we swallow that race and let the other
        // login's value stand rather than fail a valid sign-in.
        user.LastLoginAt = DateTime.UtcNow;
        try
        {
            await _db.SaveChangesAsync(ct);
        }
        catch (DbUpdateConcurrencyException)
        {
            // A parallel login for this user already stamped the row; nothing more to do.
        }

        var allOrgDtos = memberships.Select(m => new OrganizationMembershipDto(
            m.OrganizationId, m.Organization.Name, m.Organization.Slug, m.RoleId, m.Role.Name,
            m.RoleId == selected.RoleId ? permissions : Array.Empty<string>())).ToList();

        var dto = new AuthResultDto(
            accessToken.Token,
            accessToken.ExpiresAt,
            refreshTokenValue,
            refreshToken.ExpiresAt,
            new UserDto(user.Id, user.Email, user.FirstName, user.LastName, user.AvatarUrl, user.IsEmailVerified),
            allOrgDtos);

        return Result<AuthResultDto>.Ok(dto);
    }
}
