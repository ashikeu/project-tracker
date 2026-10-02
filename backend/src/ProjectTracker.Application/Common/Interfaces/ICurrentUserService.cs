namespace ProjectTracker.Application.Common.Interfaces;

/// <summary>Reads the authenticated caller's identity from the current request's JWT claims.
/// This is the ONLY source of truth for UserId/OrganizationId/Permissions in the Application
/// layer — handlers must never accept these values from request bodies or query strings.</summary>
public interface ICurrentUserService
{
    Guid? UserId { get; }
    Guid? OrganizationId { get; }
    IReadOnlyList<string> Permissions { get; }
    bool IsAuthenticated { get; }
    bool HasPermission(string permissionKey);
}
