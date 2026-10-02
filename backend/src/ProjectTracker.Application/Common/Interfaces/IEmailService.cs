namespace ProjectTracker.Application.Common.Interfaces;

public interface IEmailService
{
    Task SendEmailVerificationAsync(string toEmail, string toName, string verificationToken, CancellationToken ct);
    Task SendPasswordResetAsync(string toEmail, string toName, string resetToken, CancellationToken ct);
    Task SendOrganizationInvitationAsync(string toEmail, string organizationName, string invitedByName, CancellationToken ct);
}
