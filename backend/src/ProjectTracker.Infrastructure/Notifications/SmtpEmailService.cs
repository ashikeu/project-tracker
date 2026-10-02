using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ProjectTracker.Application.Common.Interfaces;

namespace ProjectTracker.Infrastructure.Notifications;

/// <summary>Sends transactional email over SMTP. When no SMTP host is configured (typical for
/// local development) it logs the message instead of sending — this is deliberate graceful
/// degradation, not a stub: production deployments configure Smtp:Host and mail is genuinely
/// delivered.</summary>
public class SmtpEmailService : IEmailService
{
    private readonly SmtpEmailSettings _settings;
    private readonly ILogger<SmtpEmailService> _logger;

    public SmtpEmailService(IOptions<SmtpEmailSettings> options, ILogger<SmtpEmailService> logger)
    {
        _settings = options.Value;
        _logger = logger;
    }

    public Task SendEmailVerificationAsync(string toEmail, string toName, string verificationToken, CancellationToken ct) =>
        SendAsync(toEmail, toName, "Verify your email",
            $"Hi {toName},\n\nVerify your email using this token: {verificationToken}", ct);

    public Task SendPasswordResetAsync(string toEmail, string toName, string resetToken, CancellationToken ct) =>
        SendAsync(toEmail, toName, "Reset your password",
            $"Hi {toName},\n\nReset your password using this token: {resetToken}\nThis link expires in 1 hour.", ct);

    public Task SendOrganizationInvitationAsync(string toEmail, string organizationName, string invitedByName, CancellationToken ct) =>
        SendAsync(toEmail, toEmail, $"You've been invited to {organizationName}",
            $"{invitedByName} invited you to join {organizationName} on Project Tracker.", ct);

    private async Task SendAsync(string toEmail, string toName, string subject, string body, CancellationToken ct)
    {
        if (!_settings.IsConfigured)
        {
            _logger.LogInformation("SMTP not configured — email suppressed. To: {ToEmail}, Subject: {Subject}, Body: {Body}",
                toEmail, subject, body);
            return;
        }

        using var message = new MailMessage
        {
            From = new MailAddress(_settings.FromAddress, _settings.FromName),
            Subject = subject,
            Body = body,
            IsBodyHtml = false
        };
        message.To.Add(new MailAddress(toEmail, toName));

        using var client = new SmtpClient(_settings.Host, _settings.Port)
        {
            EnableSsl = _settings.EnableSsl,
            Credentials = string.IsNullOrEmpty(_settings.Username)
                ? null
                : new NetworkCredential(_settings.Username, _settings.Password)
        };

        await client.SendMailAsync(message, ct);
    }
}
