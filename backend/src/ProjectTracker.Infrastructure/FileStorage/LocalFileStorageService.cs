using Microsoft.Extensions.Options;
using ProjectTracker.Application.Common.Interfaces;

namespace ProjectTracker.Infrastructure.FileStorage;

/// <summary>Filesystem-backed IFileStorageService for local/dev. Files are written under
/// RootPath in date-sharded subfolders with a server-generated, collision-free name; the returned
/// StoragePath is relative to RootPath and is the only thing persisted. Reads/deletes re-resolve
/// against RootPath and refuse any path that escapes it (defense-in-depth — the stored paths are
/// server-generated, but we never trust a path to stay inside the root without checking).</summary>
public class LocalFileStorageService : IFileStorageService
{
    private readonly string _root;

    public LocalFileStorageService(IOptions<FileStorageSettings> settings)
    {
        var configured = settings.Value.RootPath;
        // Relative paths resolve against the app's working directory (its content root at runtime).
        _root = Path.IsPathRooted(configured)
            ? configured
            : Path.Combine(Directory.GetCurrentDirectory(), configured);
        Directory.CreateDirectory(_root);
    }

    public async Task<string> SaveAsync(Stream content, string originalFileName, string contentType, CancellationToken ct)
    {
        var ext = Path.GetExtension(originalFileName);
        var relativeDir = Path.Combine(DateTime.UtcNow.ToString("yyyy"), DateTime.UtcNow.ToString("MM"));
        var storedName = $"{Guid.NewGuid():N}{ext}";
        var relativePath = Path.Combine(relativeDir, storedName);

        var absoluteDir = Path.Combine(_root, relativeDir);
        Directory.CreateDirectory(absoluteDir);

        var absolutePath = Path.Combine(_root, relativePath);
        await using (var target = new FileStream(absolutePath, FileMode.CreateNew, FileAccess.Write, FileShare.None))
        {
            await content.CopyToAsync(target, ct);
        }

        // Store with forward slashes so the key is stable across OSes.
        return relativePath.Replace('\\', '/');
    }

    public Task<Stream> OpenReadAsync(string storagePath, CancellationToken ct)
    {
        var absolutePath = ResolveWithinRoot(storagePath);
        if (!File.Exists(absolutePath)) throw new FileNotFoundException("Stored file not found.", storagePath);

        Stream stream = new FileStream(absolutePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        return Task.FromResult(stream);
    }

    public Task DeleteAsync(string storagePath, CancellationToken ct)
    {
        var absolutePath = ResolveWithinRoot(storagePath);
        if (File.Exists(absolutePath)) File.Delete(absolutePath);
        return Task.CompletedTask;
    }

    private string ResolveWithinRoot(string storagePath)
    {
        var absolutePath = Path.GetFullPath(Path.Combine(_root, storagePath));
        var rootFull = Path.GetFullPath(_root);
        if (!absolutePath.StartsWith(rootFull + Path.DirectorySeparatorChar, StringComparison.Ordinal)
            && !absolutePath.Equals(rootFull, StringComparison.Ordinal))
        {
            throw new InvalidOperationException("Resolved storage path escapes the storage root.");
        }
        return absolutePath;
    }
}
