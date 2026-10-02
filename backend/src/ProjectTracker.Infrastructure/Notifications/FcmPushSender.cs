using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ProjectTracker.Application.Common.Interfaces;

namespace ProjectTracker.Infrastructure.Notifications;

/// <summary>Firebase Cloud Messaging push sender. Delivery to a device needs a registered device
/// token, which the mobile client supplies in Phase 10; until device-token registration exists (and
/// FCM credentials are configured), this logs the payload it would send rather than pretending to
/// deliver — the same log-when-unconfigured behavior SmtpEmailService uses. This is the single seam
/// where real FCM dispatch is wired in.</summary>
public class FcmPushSender : IPushSender
{
    private readonly FcmSettings _settings;
    private readonly ILogger<FcmPushSender> _logger;

    public FcmPushSender(IOptions<FcmSettings> settings, ILogger<FcmPushSender> logger)
    {
        _settings = settings.Value;
        _logger = logger;
    }

    public Task SendAsync(Guid userId, string title, string body, IReadOnlyDictionary<string, string> data, CancellationToken ct)
    {
        if (!_settings.IsConfigured)
        {
            _logger.LogInformation("[Push:dev] → user {UserId}: {Title} — {Body}", userId, title, body);
            return Task.CompletedTask;
        }

        // FCM is configured, but there are no device tokens to target until the mobile client
        // registers them (Phase 10). Log at warning so a misconfiguration here is visible rather
        // than silently dropped.
        _logger.LogWarning("FCM is configured but no device-token registration exists yet (Phase 10); " +
                           "skipping push to user {UserId}.", userId);
        return Task.CompletedTask;
    }
}
