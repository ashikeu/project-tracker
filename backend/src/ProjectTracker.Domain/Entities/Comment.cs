using ProjectTracker.Domain.Common;

namespace ProjectTracker.Domain.Entities;

/// <summary>A comment on a Task. Threaded one level via ParentCommentId (a reply points at a
/// top-level comment). Soft-deleted so reply threads and mention history stay intact.</summary>
public class Comment : BaseSoftDeleteEntity, ITenantEntity
{
    /// <summary>Denormalized from Task.OrganizationId — see Epic.OrganizationId for the rationale.</summary>
    public Guid OrganizationId { get; set; }

    public Guid TaskId { get; set; }
    public TaskItem Task { get; set; } = null!;

    public Guid AuthorUserId { get; set; }
    public User Author { get; set; } = null!;

    public string Body { get; set; } = string.Empty;

    /// <summary>Set on a reply; null for a top-level comment.</summary>
    public Guid? ParentCommentId { get; set; }
    public Comment? ParentComment { get; set; }
    public ICollection<Comment> Replies { get; set; } = new List<Comment>();

    public bool IsEdited { get; set; }
    public DateTime? EditedAt { get; set; }

    public ICollection<CommentMention> Mentions { get; set; } = new List<CommentMention>();
}

/// <summary>Join row recording that a comment @-mentioned a user. Drives mention notifications
/// (generated in Phase 8).</summary>
public class CommentMention
{
    public Guid CommentId { get; set; }
    public Comment Comment { get; set; } = null!;

    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
}
