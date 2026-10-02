using ProjectTracker.Domain.Common;
using ProjectTracker.Domain.Enums;

namespace ProjectTracker.Domain.Entities;

/// <summary>Metadata for an uploaded file. The bytes live on IFileStorageService at StoragePath;
/// this row is the authoritative record and is hard-deleted (with its file) rather than
/// soft-deleted, so storage doesn't accumulate orphaned blobs.</summary>
public class Attachment : BaseAuditableEntity, ITenantEntity
{
    public Guid OrganizationId { get; set; }

    /// <summary>Polymorphic owner — see AttachmentEntityType.</summary>
    public AttachmentEntityType EntityType { get; set; }
    public Guid EntityId { get; set; }

    /// <summary>Server-side stored name (unique, collision-free); OriginalFileName is what the
    /// user uploaded and what downloads are named as.</summary>
    public string FileName { get; set; } = string.Empty;
    public string OriginalFileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long FileSize { get; set; }

    /// <summary>Opaque key handed back by IFileStorageService.SaveAsync; never a client-supplied path.</summary>
    public string StoragePath { get; set; } = string.Empty;

    public Guid UploadedByUserId { get; set; }
    public User UploadedBy { get; set; } = null!;

    public DateTime UploadedAt { get; set; }
}
