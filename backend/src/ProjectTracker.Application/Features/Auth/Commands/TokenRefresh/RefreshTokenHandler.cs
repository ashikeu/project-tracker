using Microsoft.EntityFrameworkCore;
using ProjectTracker.Application.Common.Interfaces;
using ProjectTracker.Application.Features.Auth.Dtos;
using ProjectTracker.Domain.Entities;
using ProjectTracker.Domain.Enums;
using ProjectTracker.Shared.Results;

namespace ProjectTracker.Application.Features.Auth.Commands.TokenRefresh;

public interface IRefreshTokenHandler
{
    Task<Result<AuthResultDto>> Handle(RefreshTokenCommand command, CancellationToken ct);
}

/// <summary>Implements refresh-token rotation: the presented token is revoked and replaced by a
/// newly issued one on every use. Reuse of an already-revoked token revokes the whole chain
/// (signals a stolen token) and forces re-authentication.</summary>
public class RefreshTokenHandler : IRefreshTokenHandler
{
    private readonly IApplicationDbContext _db;
    private readonly ITokenHasher _tokenHasher;
    private readonly IJwtTokenService _tokenService;

    public RefreshTokenHandler(IApplicationDbContext db, ITokenHasher tokenHasher, IJwtTokenService tokenService)
    {
        _db = db;
        _tokenHasher = tokenHasher;
        _tokenService = tokenService;
    }

    public async Task<Result<AuthResultDto>> Handle(RefreshTokenCommand command, CancellationToken ct)
    {
        var hash = _tokenHasher.Hash(command.RefreshToken);
        var existing = await _db.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.TokenHash == hash, ct);

        if (existing is null)
        {
            return Result<AuthResultDto>.Fail("Invalid refresh token.");
        }

        if (existing.RevokedAt is not null)
        {
            // Reused a token already rotated away — treat as compromised, revoke all active
            // tokens for this user.
            var activeTokens = await _db.RefreshTokens
                .Where(rt => rt.UserId == existing.UserId && rt.RevokedAt == null)
                .ToListAsync(ct);
            foreach (var t in activeTokens) t.RevokedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync(ct);
            return Result<AuthResultDto>.Fail("This refresh token has already been used. All sessions have been revoked; please log in again.");
        }

        if (existing.ExpiresAt <= DateTime.UtcNow)
        {
            return Result<AuthResultDto>.Fail("Refresh token has expired.");
        }

        var user = existing.User;
        if (!user.IsActive)
        {
            return Result<AuthResultDto>.Fail("This account has been deactivated.");
        }

        // See LoginHandler for why IgnoreQueryFilters is correct here.
        var memberships = await _db.OrganizationUsers
            .IgnoreQueryFilters()
            .Where(ou => ou.UserId == user.Id && ou.Status == OrganizationUserStatus.Active)
            .Include(ou => ou.Organization)
            .Include(ou => ou.Role)
            .ToListAsync(ct);

        var selected = memberships.FirstOrDefault();
        if (selected is null)
        {
            return Result<AuthResultDto>.Fail("This account is not a member of any active organization.");
        }

        var permissions = await _db.RolePermissions.AsNoTracking()
            .Where(rp => rp.RoleId == selected.RoleId)
            .Select(rp => rp.Permission.Key)
            .ToListAsync(ct);

        var accessToken = _tokenService.IssueAccessToken(user, selected.Organization, selected.Role, permissions);
        var newRefreshTokenValue = _tokenService.GenerateRefreshTokenValue();
        var newRefreshToken = new RefreshToken
        {
            User = user,
            TokenHash = _tokenHasher.Hash(newRefreshTokenValue),
            ExpiresAt = DateTime.UtcNow.AddDays(30),
            CreatedByIp = command.IpAddress
        };
        _db.RefreshTokens.Add(newRefreshToken);

        existing.RevokedAt = DateTime.UtcNow;
        existing.ReplacedByTokenId = newRefreshToken.Id;

        await _db.SaveChangesAsync(ct);

        var dto = new AuthResultDto(
            accessToken.Token,
            accessToken.ExpiresAt,
            newRefreshTokenValue,
            newRefreshToken.ExpiresAt,
            new UserDto(user.Id, user.Email, user.FirstName, user.LastName, user.AvatarUrl, user.IsEmailVerified),
            memberships.Select(m => new OrganizationMembershipDto(
                m.OrganizationId, m.Organization.Name, m.Organization.Slug, m.RoleId, m.Role.Name,
                m.RoleId == selected.RoleId ? permissions : Array.Empty<string>())).ToList());

        return Result<AuthResultDto>.Ok(dto);
    }
}
