using ProjectTracker.Domain.Common;

namespace ProjectTracker.Domain.Entities;

public class User : BaseAuditableEntity
{
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? AvatarUrl { get; set; }

    public bool IsEmailVerified { get; set; }
    public string? EmailVerificationToken { get; set; }
    public DateTime? EmailVerificationTokenExpiresAt { get; set; }

    public string? PasswordResetToken { get; set; }
    public DateTime? PasswordResetTokenExpiresAt { get; set; }

    public bool IsActive { get; set; } = true;
    public DateTime? LastLoginAt { get; set; }

    /// <summary>JSON blob for UI/notification preferences that don't warrant their own tables.</summary>
    public string? PreferencesJson { get; set; }

    public ICollection<OrganizationUser> OrganizationMemberships { get; set; } = new List<OrganizationUser>();
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();

    public string FullName => $"{FirstName} {LastName}".Trim();
}
