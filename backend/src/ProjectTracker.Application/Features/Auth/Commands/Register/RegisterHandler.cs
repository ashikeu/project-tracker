using Microsoft.EntityFrameworkCore;
using ProjectTracker.Application.Common.Interfaces;
using ProjectTracker.Application.Features.Auth.Dtos;
using ProjectTracker.Domain.Entities;
using ProjectTracker.Shared.Constants;
using ProjectTracker.Shared.Results;

namespace ProjectTracker.Application.Features.Auth.Commands.Register;

public interface IRegisterHandler
{
    Task<Result<AuthResultDto>> Handle(RegisterCommand command, CancellationToken ct);
}

/// <summary>Registers a new user together with a brand-new Organization (the caller becomes
/// OrganizationOwner). Joining an existing organization happens separately via an accepted
/// invitation (see Features/Organizations/Commands/AcceptInvitation).</summary>
public class RegisterHandler : IRegisterHandler
{
    private readonly IApplicationDbContext _db;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenHasher _tokenHasher;
    private readonly IJwtTokenService _tokenService;
    private readonly IEmailService _emailService;

    public RegisterHandler(
        IApplicationDbContext db,
        IPasswordHasher passwordHasher,
        ITokenHasher tokenHasher,
        IJwtTokenService tokenService,
        IEmailService emailService)
    {
        _db = db;
        _passwordHasher = passwordHasher;
        _tokenHasher = tokenHasher;
        _tokenService = tokenService;
        _emailService = emailService;
    }

    public async Task<Result<AuthResultDto>> Handle(RegisterCommand command, CancellationToken ct)
    {
        var normalizedEmail = command.Email.Trim().ToLowerInvariant();

        var emailExists = await _db.Users.AsNoTracking()
            .AnyAsync(u => u.Email == normalizedEmail, ct);
        if (emailExists)
        {
            return Result<AuthResultDto>.FailValidation(new[]
            {
                new ValidationError(nameof(command.Email), "An account with this email already exists.")
            });
        }

        var user = new User
        {
            Email = normalizedEmail,
            PasswordHash = _passwordHasher.Hash(command.Password),
            FirstName = command.FirstName.Trim(),
            LastName = command.LastName.Trim(),
            IsActive = true,
            IsEmailVerified = false,
            EmailVerificationToken = Guid.NewGuid().ToString("N"),
            EmailVerificationTokenExpiresAt = DateTime.UtcNow.AddDays(3),
            CreatedAt = DateTime.UtcNow
        };

        var slug = await GenerateUniqueSlugAsync(command.OrganizationName, ct);
        var organization = new Organization
        {
            Name = command.OrganizationName.Trim(),
            Slug = slug,
            CreatedAt = DateTime.UtcNow
        };

        var ownerRole = await _db.Roles.AsNoTracking()
            .FirstOrDefaultAsync(r => r.IsSystemRole && r.Name == SystemRoles.OrganizationOwner, ct);
        if (ownerRole is null)
        {
            return Result<AuthResultDto>.Fail("System roles are not seeded. Contact an administrator.");
        }

        var membership = new OrganizationUser
        {
            Organization = organization,
            User = user,
            RoleId = ownerRole.Id,
            Status = Domain.Enums.OrganizationUserStatus.Active,
            JoinedAt = DateTime.UtcNow
        };

        _db.Organizations.Add(organization);
        _db.Users.Add(user);
        _db.OrganizationUsers.Add(membership);

        var permissions = await _db.RolePermissions.AsNoTracking()
            .Where(rp => rp.RoleId == ownerRole.Id)
            .Select(rp => rp.Permission.Key)
            .ToListAsync(ct);

        var accessToken = _tokenService.IssueAccessToken(user, organization, ownerRole, permissions);
        var refreshTokenValue = _tokenService.GenerateRefreshTokenValue();
        var refreshToken = new RefreshToken
        {
            User = user,
            TokenHash = _tokenHasher.Hash(refreshTokenValue),
            ExpiresAt = DateTime.UtcNow.AddDays(30)
        };
        _db.RefreshTokens.Add(refreshToken);

        await _db.SaveChangesAsync(ct);

        await _emailService.SendEmailVerificationAsync(user.Email, user.FullName, user.EmailVerificationToken!, ct);

        var dto = new AuthResultDto(
            accessToken.Token,
            accessToken.ExpiresAt,
            refreshTokenValue,
            refreshToken.ExpiresAt,
            new UserDto(user.Id, user.Email, user.FirstName, user.LastName, user.AvatarUrl, user.IsEmailVerified),
            new[]
            {
                new OrganizationMembershipDto(organization.Id, organization.Name, organization.Slug, ownerRole.Id, ownerRole.Name, permissions)
            });

        return Result<AuthResultDto>.Ok(dto, "Registration successful.");
    }

    private async Task<string> GenerateUniqueSlugAsync(string organizationName, CancellationToken ct)
    {
        var baseSlug = Slugify(organizationName);
        var slug = baseSlug;
        var suffix = 1;
        while (await _db.Organizations.AsNoTracking().AnyAsync(o => o.Slug == slug, ct))
        {
            slug = $"{baseSlug}-{++suffix}";
        }
        return slug;
    }

    private static string Slugify(string value)
    {
        var lowered = value.Trim().ToLowerInvariant();
        var chars = lowered.Select(c => char.IsLetterOrDigit(c) ? c : '-').ToArray();
        var collapsed = new string(chars);
        while (collapsed.Contains("--")) collapsed = collapsed.Replace("--", "-");
        return collapsed.Trim('-');
    }
}
