namespace ProjectTracker.Application.Features.Comments.Dtos;

public record MentionDto(Guid UserId, string Name);

/// <summary>Threaded one level: top-level comments carry their Replies inline; a reply's own
/// Replies list is always empty (a subtask/reply cannot itself be replied to).</summary>
public record CommentDto(
    Guid Id, Guid TaskId, Guid AuthorUserId, string AuthorName, string Body,
    Guid? ParentCommentId, bool IsEdited, DateTime? EditedAt, DateTime CreatedAt,
    IReadOnlyList<MentionDto> Mentions, IReadOnlyList<CommentDto> Replies);
