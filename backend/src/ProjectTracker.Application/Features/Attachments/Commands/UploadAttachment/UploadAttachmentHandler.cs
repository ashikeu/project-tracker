using Microsoft.EntityFrameworkCore;
using ProjectTracker.Application.Common.Interfaces;
using ProjectTracker.Application.Features.Attachments.Dtos;
using ProjectTracker.Domain.Entities;
using ProjectTracker.Domain.Enums;
using ProjectTracker.Shared.Results;

namespace ProjectTracker.Application.Features.Attachments.Commands.UploadAttachment;

public record UploadAttachmentCommand(
    AttachmentEntityType EntityType, Guid EntityId,
    Stream Content, string OriginalFileName, string ContentType, long Length);

public interface IUploadAttachmentHandler
{
    Task<Result<AttachmentDto>> Handle(UploadAttachmentCommand command, CancellationToken ct);
}

public class UploadAttachmentHandler : IUploadAttachmentHandler
{
    private readonly IApplicationDbContext _db;
    private readonly IFileStorageService _storage;
    private readonly ICurrentUserService _currentUser;
    private readonly IFileUploadPolicy _policy;

    public UploadAttachmentHandler(IApplicationDbContext db, IFileStorageService storage,
        ICurrentUserService currentUser, IFileUploadPolicy policy)
    {
        _db = db;
        _storage = storage;
        _currentUser = currentUser;
        _policy = policy;
    }

    public async Task<Result<AttachmentDto>> Handle(UploadAttachmentCommand command, CancellationToken ct)
    {
        if (command.Length <= 0)
        {
            return Result<AttachmentDto>.FailValidation(new[] { new ValidationError(nameof(command.Content), "The uploaded file is empty.") });
        }
        if (command.Length > _policy.MaxFileSizeBytes)
        {
            return Result<AttachmentDto>.FailValidation(new[] { new ValidationError(nameof(command.Content), $"File exceeds the maximum allowed size of {_policy.MaxFileSizeBytes / (1024 * 1024)} MB.") });
        }
        if (!_policy.IsContentTypeAllowed(command.ContentType))
        {
            return Result<AttachmentDto>.FailValidation(new[] { new ValidationError(nameof(command.ContentType), $"Content type '{command.ContentType}' is not allowed.") });
        }

        // Verify the owning entity exists (and, via the tenant query filter, belongs to the caller's org).
        var orgId = await ResolveOwnerOrganizationAsync(command.EntityType, command.EntityId, ct);
        if (orgId is null)
        {
            return Result<AttachmentDto>.Fail($"{command.EntityType} not found.");
        }

        var storagePath = await _storage.SaveAsync(command.Content, command.OriginalFileName, command.ContentType, ct);

        var attachment = new Attachment
        {
            OrganizationId = orgId.Value,
            EntityType = command.EntityType,
            EntityId = command.EntityId,
            FileName = System.IO.Path.GetFileName(storagePath),
            OriginalFileName = command.OriginalFileName,
            ContentType = command.ContentType,
            FileSize = command.Length,
            StoragePath = storagePath,
            UploadedByUserId = _currentUser.UserId!.Value,
            UploadedAt = DateTime.UtcNow
        };
        _db.Attachments.Add(attachment);

        try
        {
            await _db.SaveChangesAsync(ct);
        }
        catch
        {
            // Don't leak a stored blob if the metadata insert fails.
            await _storage.DeleteAsync(storagePath, ct);
            throw;
        }

        var name = await _db.Users.AsNoTracking().Where(u => u.Id == attachment.UploadedByUserId)
            .Select(u => u.FirstName + " " + u.LastName).FirstOrDefaultAsync(ct) ?? string.Empty;

        return Result<AttachmentDto>.Ok(new AttachmentDto(
            attachment.Id, attachment.EntityType.ToString(), attachment.EntityId, attachment.OriginalFileName,
            attachment.ContentType, attachment.FileSize, attachment.UploadedByUserId, name, attachment.UploadedAt));
    }

    private async Task<Guid?> ResolveOwnerOrganizationAsync(AttachmentEntityType type, Guid id, CancellationToken ct) => type switch
    {
        AttachmentEntityType.Task => await _db.Tasks.AsNoTracking().Where(t => t.Id == id).Select(t => (Guid?)t.OrganizationId).FirstOrDefaultAsync(ct),
        AttachmentEntityType.Comment => await _db.Comments.AsNoTracking().Where(c => c.Id == id).Select(c => (Guid?)c.OrganizationId).FirstOrDefaultAsync(ct),
        AttachmentEntityType.Project => await _db.Projects.AsNoTracking().Where(p => p.Id == id).Select(p => (Guid?)p.OrganizationId).FirstOrDefaultAsync(ct),
        AttachmentEntityType.Epic => await _db.Epics.AsNoTracking().Where(e => e.Id == id).Select(e => (Guid?)e.OrganizationId).FirstOrDefaultAsync(ct),
        _ => null
    };
}
