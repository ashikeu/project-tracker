namespace ProjectTracker.Application.Features.Auth.Dtos;

public record UserDto(Guid Id, string Email, string FirstName, string LastName, string? AvatarUrl, bool IsEmailVerified);

public record OrganizationMembershipDto(Guid OrganizationId, string OrganizationName, string OrganizationSlug, Guid RoleId, string RoleName, IReadOnlyList<string> Permissions);

public record AuthResultDto(
    string AccessToken,
    DateTime AccessTokenExpiresAt,
    string RefreshToken,
    DateTime RefreshTokenExpiresAt,
    UserDto User,
    IReadOnlyList<OrganizationMembershipDto> Organizations);
