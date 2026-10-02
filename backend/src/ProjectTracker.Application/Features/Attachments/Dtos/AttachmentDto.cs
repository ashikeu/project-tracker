namespace ProjectTracker.Application.Features.Attachments.Dtos;

public record AttachmentDto(
    Guid Id, string EntityType, Guid EntityId, string OriginalFileName, string ContentType,
    long FileSize, Guid UploadedByUserId, string UploadedByName, DateTime UploadedAt);

/// <summary>Carries a stored file's bytes plus the metadata a controller needs to stream it back.</summary>
public record AttachmentDownload(Stream Content, string ContentType, string FileName);
