using System.Linq.Expressions;
using ProjectTracker.Application.Features.Comments.Dtos;
using ProjectTracker.Domain.Entities;

namespace ProjectTracker.Application.Features.Comments.Mappings;

public static class CommentProjection
{
    /// <summary>EF-translatable projection of a Comment to its DTO, including mentions and (for a
    /// top-level comment) its one level of replies. The soft-delete global query filter is applied
    /// to the Replies subquery automatically, so deleted replies never appear.</summary>
    public static readonly Expression<Func<Comment, CommentDto>> ToDto = c => new CommentDto(
        c.Id, c.TaskId, c.AuthorUserId, c.Author.FirstName + " " + c.Author.LastName, c.Body,
        c.ParentCommentId, c.IsEdited, c.EditedAt, c.CreatedAt,
        c.Mentions.Select(m => new MentionDto(m.UserId, m.User.FirstName + " " + m.User.LastName)).ToList(),
        c.Replies
            .OrderBy(r => r.CreatedAt)
            .Select(r => new CommentDto(
                r.Id, r.TaskId, r.AuthorUserId, r.Author.FirstName + " " + r.Author.LastName, r.Body,
                r.ParentCommentId, r.IsEdited, r.EditedAt, r.CreatedAt,
                r.Mentions.Select(m => new MentionDto(m.UserId, m.User.FirstName + " " + m.User.LastName)).ToList(),
                new List<CommentDto>()))
            .ToList());
}
