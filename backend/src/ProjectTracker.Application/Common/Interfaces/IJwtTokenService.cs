using ProjectTracker.Domain.Entities;

namespace ProjectTracker.Application.Common.Interfaces;

public record AccessTokenResult(string Token, DateTime ExpiresAt);

public interface IJwtTokenService
{
    /// <summary>Issues a short-lived JWT access token carrying UserId, OrganizationId, Role and
    /// flattened permission-key claims for the given organization membership.</summary>
    AccessTokenResult IssueAccessToken(User user, Organization organization, Role role, IReadOnlyList<string> permissions);

    /// <summary>Generates a cryptographically random opaque refresh token (the raw value —
    /// callers persist only its hash via IPasswordHasher/hashing helper).</summary>
    string GenerateRefreshTokenValue();
}
