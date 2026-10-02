namespace ProjectTracker.Application.Features.Organizations.Dtos;

public record OrganizationDto(Guid Id, string Name, string Slug, string Status, DateTime CreatedAt);

public record OrganizationMemberDto(
    Guid OrganizationUserId, Guid UserId, string Email, string FirstName, string LastName,
    string? AvatarUrl, Guid RoleId, string RoleName, string Status, DateTime? JoinedAt);

public record RoleDto(Guid Id, string Name, bool IsSystemRole, IReadOnlyList<string> Permissions);
