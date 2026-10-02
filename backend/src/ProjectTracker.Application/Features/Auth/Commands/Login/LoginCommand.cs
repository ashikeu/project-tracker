namespace ProjectTracker.Application.Features.Auth.Commands.Login;

/// <summary>OrganizationId is optional: if the user belongs to exactly one active organization
/// it is auto-selected; if they belong to several, the caller must pass the one to sign into
/// (the client obtains the list from a prior failed attempt's Result.Errors / a dedicated
/// "my-organizations" lookup before the org exists in this MVP — see tasks.md Phase 3+ for a
/// dedicated organization-switch endpoint).</summary>
public record LoginCommand(string Email, string Password, Guid? OrganizationId, string? IpAddress);
