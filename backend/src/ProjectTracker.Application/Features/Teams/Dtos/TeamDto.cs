namespace ProjectTracker.Application.Features.Teams.Dtos;

public record TeamDto(
    Guid Id, string Name, string? Description, Guid? LeadUserId, string? LeadName,
    bool IsArchived, int MemberCount, DateTime CreatedAt);

public record TeamMemberDto(
    Guid TeamMemberId, Guid UserId, string Email, string FirstName, string LastName,
    string? AvatarUrl, string Role);
