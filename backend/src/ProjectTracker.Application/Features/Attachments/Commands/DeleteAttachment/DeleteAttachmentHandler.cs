using Microsoft.EntityFrameworkCore;
using ProjectTracker.Application.Common.Interfaces;
using ProjectTracker.Shared.Results;

namespace ProjectTracker.Application.Features.Attachments.Commands.DeleteAttachment;

public record DeleteAttachmentCommand(Guid AttachmentId);

public interface IDeleteAttachmentHandler
{
    Task<Result> Handle(DeleteAttachmentCommand command, CancellationToken ct);
}

public class DeleteAttachmentHandler : IDeleteAttachmentHandler
{
    private readonly IApplicationDbContext _db;
    private readonly IFileStorageService _storage;

    public DeleteAttachmentHandler(IApplicationDbContext db, IFileStorageService storage)
    {
        _db = db;
        _storage = storage;
    }

    public async Task<Result> Handle(DeleteAttachmentCommand command, CancellationToken ct)
    {
        var attachment = await _db.Attachments.FirstOrDefaultAsync(a => a.Id == command.AttachmentId, ct);
        if (attachment is null) return Result.Fail("Attachment not found.");

        // Remove the row first; only delete the blob once the DB change is committed, so a storage
        // failure can't leave a dangling metadata row pointing at a deleted file.
        _db.Attachments.Remove(attachment);
        await _db.SaveChangesAsync(ct);
        await _storage.DeleteAsync(attachment.StoragePath, ct);

        return Result.Ok("Attachment deleted.");
    }
}
