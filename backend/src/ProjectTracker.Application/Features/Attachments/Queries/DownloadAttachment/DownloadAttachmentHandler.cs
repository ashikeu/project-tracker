using Microsoft.EntityFrameworkCore;
using ProjectTracker.Application.Common.Interfaces;
using ProjectTracker.Application.Features.Attachments.Dtos;
using ProjectTracker.Shared.Results;

namespace ProjectTracker.Application.Features.Attachments.Queries.DownloadAttachment;

public record DownloadAttachmentQuery(Guid AttachmentId);

public interface IDownloadAttachmentHandler
{
    Task<Result<AttachmentDownload>> Handle(DownloadAttachmentQuery query, CancellationToken ct);
}

public class DownloadAttachmentHandler : IDownloadAttachmentHandler
{
    private readonly IApplicationDbContext _db;
    private readonly IFileStorageService _storage;

    public DownloadAttachmentHandler(IApplicationDbContext db, IFileStorageService storage)
    {
        _db = db;
        _storage = storage;
    }

    public async Task<Result<AttachmentDownload>> Handle(DownloadAttachmentQuery query, CancellationToken ct)
    {
        // Tenant query filter ensures a caller can only resolve attachments in their own org.
        var attachment = await _db.Attachments.AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == query.AttachmentId, ct);
        if (attachment is null) return Result<AttachmentDownload>.Fail("Attachment not found.");

        var stream = await _storage.OpenReadAsync(attachment.StoragePath, ct);
        return Result<AttachmentDownload>.Ok(new AttachmentDownload(stream, attachment.ContentType, attachment.OriginalFileName));
    }
}
