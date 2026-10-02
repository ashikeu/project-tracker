namespace ProjectTracker.Application.Features.Comments.Commands.CreateComment;

/// <summary>MentionedUserIds are supplied explicitly by the client (from its @-picker) rather than
/// parsed out of Body — keeps mention resolution unambiguous and server-validatable.</summary>
public record CreateCommentCommand(Guid TaskId, string Body, Guid? ParentCommentId, IReadOnlyList<Guid>? MentionedUserIds);
