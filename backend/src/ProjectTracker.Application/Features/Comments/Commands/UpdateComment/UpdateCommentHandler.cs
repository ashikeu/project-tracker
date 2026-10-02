using Microsoft.EntityFrameworkCore;
using ProjectTracker.Application.Common.Interfaces;
using ProjectTracker.Application.Features.Comments.Dtos;
using ProjectTracker.Application.Features.Comments.Mappings;
using ProjectTracker.Domain.Entities;
using ProjectTracker.Shared.Results;

namespace ProjectTracker.Application.Features.Comments.Commands.UpdateComment;

public record UpdateCommentCommand(Guid CommentId, string Body, IReadOnlyList<Guid>? MentionedUserIds);

public interface IUpdateCommentHandler
{
    Task<Result<CommentDto>> Handle(UpdateCommentCommand command, CancellationToken ct);
}

public class UpdateCommentHandler : IUpdateCommentHandler
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public UpdateCommentHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<Result<CommentDto>> Handle(UpdateCommentCommand command, CancellationToken ct)
    {
        var comment = await _db.Comments
            .Include(c => c.Mentions)
            .FirstOrDefaultAsync(c => c.Id == command.CommentId, ct);
        if (comment is null) return Result<CommentDto>.Fail("Comment not found.");

        // Editing is author-only — even a moderator with comment.delete can remove but not rewrite
        // someone else's words.
        if (comment.AuthorUserId != _currentUser.UserId)
        {
            return Result<CommentDto>.Fail("You can only edit your own comments.");
        }

        var mentionIds = (command.MentionedUserIds ?? Array.Empty<Guid>()).Distinct().ToList();
        if (mentionIds.Count > 0)
        {
            var validCount = await _db.OrganizationUsers.AsNoTracking()
                .CountAsync(ou => ou.OrganizationId == comment.OrganizationId && mentionIds.Contains(ou.UserId), ct);
            if (validCount != mentionIds.Count)
            {
                return Result<CommentDto>.FailValidation(new[] { new ValidationError(nameof(command.MentionedUserIds), "One or more mentioned users are not members of this organization.") });
            }
        }

        comment.Body = command.Body.Trim();
        comment.IsEdited = true;
        comment.EditedAt = DateTime.UtcNow;

        _db.CommentMentions.RemoveRange(comment.Mentions);
        foreach (var userId in mentionIds)
        {
            comment.Mentions.Add(new CommentMention { CommentId = comment.Id, UserId = userId });
        }

        await _db.SaveChangesAsync(ct);

        var dto = await _db.Comments.AsNoTracking()
            .Where(c => c.Id == comment.Id).Select(CommentProjection.ToDto).FirstAsync(ct);
        return Result<CommentDto>.Ok(dto);
    }
}
