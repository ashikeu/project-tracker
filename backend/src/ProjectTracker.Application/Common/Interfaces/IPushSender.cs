namespace ProjectTracker.Application.Common.Interfaces;

/// <summary>Sends a push notification to a user's registered devices. Implemented over Firebase
/// Cloud Messaging in production; the dev implementation logs the payload when FCM isn't configured
/// (mirrors how SmtpEmailService behaves without SMTP credentials).</summary>
public interface IPushSender
{
    Task SendAsync(Guid userId, string title, string body, IReadOnlyDictionary<string, string> data, CancellationToken ct);
}
