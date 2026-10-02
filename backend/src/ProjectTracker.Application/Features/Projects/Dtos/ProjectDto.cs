namespace ProjectTracker.Application.Features.Projects.Dtos;

public record ProjectDto(
    Guid Id, string Key, string Name, string? Description,
    Guid OwnerUserId, string OwnerName, Guid? ManagerUserId, string? ManagerName,
    string Status, string Priority, DateOnly? StartDate, DateOnly? TargetDate,
    bool IsArchived, DateTime CreatedAt);

public record ProjectMemberDto(Guid ProjectMemberId, Guid UserId, string Email, string FirstName, string LastName, string? AvatarUrl, Guid RoleId, string RoleName);

public record ProjectTeamDto(Guid TeamId, string TeamName);
