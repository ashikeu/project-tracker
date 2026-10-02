namespace ProjectTracker.Infrastructure.Notifications;

public class FcmSettings
{
    public const string SectionName = "Fcm";

    /// <summary>Path to the Firebase service-account JSON. Blank in local dev — the sender then logs
    /// the payload instead of calling FCM (see FcmPushSender).</summary>
    public string? CredentialsPath { get; set; }

    /// <summary>Firebase project id the messages target.</summary>
    public string? ProjectId { get; set; }

    public bool IsConfigured => !string.IsNullOrWhiteSpace(CredentialsPath) && !string.IsNullOrWhiteSpace(ProjectId);
}
