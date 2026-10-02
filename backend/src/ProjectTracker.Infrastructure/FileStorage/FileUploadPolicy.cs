using Microsoft.Extensions.Options;
using ProjectTracker.Application.Common.Interfaces;

namespace ProjectTracker.Infrastructure.FileStorage;

public class FileUploadPolicy : IFileUploadPolicy
{
    private readonly FileStorageSettings _settings;

    public FileUploadPolicy(IOptions<FileStorageSettings> settings) => _settings = settings.Value;

    public long MaxFileSizeBytes => _settings.MaxFileSizeBytes;

    /// <summary>An empty allow-list means "allow any" (dev convenience); otherwise the type must
    /// be listed (case-insensitive).</summary>
    public bool IsContentTypeAllowed(string contentType)
    {
        if (_settings.AllowedContentTypes.Length == 0) return true;
        return _settings.AllowedContentTypes.Any(t => string.Equals(t, contentType, StringComparison.OrdinalIgnoreCase));
    }
}
