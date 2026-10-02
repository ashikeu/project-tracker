namespace ProjectTracker.Infrastructure.Notifications;

public class SmtpEmailSettings
{
    public const string SectionName = "Smtp";

    public string Host { get; set; } = string.Empty;
    public int Port { get; set; } = 587;
    public string? Username { get; set; }
    public string? Password { get; set; }
    public bool EnableSsl { get; set; } = true;
    public string FromAddress { get; set; } = "no-reply@projecttracker.local";
    public string FromName { get; set; } = "Project Tracker";

    /// <summary>Set when Host is blank — SmtpEmailService then logs instead of sending, which
    /// is the expected behavior for local development without SMTP credentials configured.</summary>
    public bool IsConfigured => !string.IsNullOrWhiteSpace(Host);
}
