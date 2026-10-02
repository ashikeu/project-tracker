namespace ProjectTracker.Infrastructure.FileStorage;

public class FileStorageSettings
{
    public const string SectionName = "FileStorage";

    /// <summary>Root directory for stored files. Relative paths resolve against the app's content
    /// root; an absolute path is used as-is. Defaults to "App_Data/uploads".</summary>
    public string RootPath { get; set; } = "App_Data/uploads";

    /// <summary>Hard cap enforced on every upload. Default 25 MB.</summary>
    public long MaxFileSizeBytes { get; set; } = 25 * 1024 * 1024;

    /// <summary>Allow-list of accepted MIME types. Empty = allow any (dev convenience); populate
    /// in production to harden uploads (see Phase 12).</summary>
    public string[] AllowedContentTypes { get; set; } = Array.Empty<string>();
}
