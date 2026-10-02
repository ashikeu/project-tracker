using Microsoft.EntityFrameworkCore;
using ProjectTracker.Application.Common.Interfaces;
using ProjectTracker.Shared.Constants;
using ProjectTracker.Shared.Results;

namespace ProjectTracker.Application.Features.Comments.Commands.DeleteComment;

public record DeleteCommentCommand(Guid CommentId);

public interface IDeleteCommentHandler
{
    Task<Result> Handle(DeleteCommentCommand command, CancellationToken ct);
}

public class DeleteCommentHandler : IDeleteCommentHandler
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public DeleteCommentHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<Result> Handle(DeleteCommentCommand command, CancellationToken ct)
    {
        var comment = await _db.Comments.FirstOrDefaultAsync(c => c.Id == command.CommentId, ct);
        if (comment is null) return Result.Fail("Comment not found.");

        // The author can always remove their own comment; anyone else needs the comment.delete
        // permission (a moderation capability).
        var isAuthor = comment.AuthorUserId == _currentUser.UserId;
        if (!isAuthor && !_currentUser.HasPermission(PermissionKeys.CommentDelete))
        {
            return Result.Fail("You do not have permission to delete this comment.");
        }

        comment.IsDeleted = true;
        await _db.SaveChangesAsync(ct);
        return Result.Ok("Comment deleted.");
    }
}
