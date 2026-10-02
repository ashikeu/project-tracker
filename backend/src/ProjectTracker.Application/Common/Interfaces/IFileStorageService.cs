namespace ProjectTracker.Application.Common.Interfaces;

/// <summary>Abstraction over blob storage so Application never touches the filesystem (or a cloud
/// bucket) directly. Infrastructure supplies the implementation (local disk for dev — see
/// LocalFileStorageService). Save returns an opaque StoragePath that later reads/deletes use;
/// callers never construct or interpret that path.</summary>
public interface IFileStorageService
{
    Task<string> SaveAsync(Stream content, string originalFileName, string contentType, CancellationToken ct);

    Task<Stream> OpenReadAsync(string storagePath, CancellationToken ct);

    Task DeleteAsync(string storagePath, CancellationToken ct);
}
