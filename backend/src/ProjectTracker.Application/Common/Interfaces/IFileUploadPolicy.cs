namespace ProjectTracker.Application.Common.Interfaces;

/// <summary>Upload constraints (max size, allowed content types) exposed to Application without it
/// depending on Infrastructure's config type. Infrastructure implements this over
/// FileStorageSettings.</summary>
public interface IFileUploadPolicy
{
    long MaxFileSizeBytes { get; }

    bool IsContentTypeAllowed(string contentType);
}
