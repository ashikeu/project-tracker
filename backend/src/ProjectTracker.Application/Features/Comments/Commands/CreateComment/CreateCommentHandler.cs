using Microsoft.EntityFrameworkCore;
using ProjectTracker.Application.Common.Interfaces;
using ProjectTracker.Application.Features.Comments.Dtos;
using ProjectTracker.Application.Features.Comments.Mappings;
using ProjectTracker.Domain.Entities;
using ProjectTracker.Domain.Enums;
using ProjectTracker.Shared.Results;

namespace ProjectTracker.Application.Features.Comments.Commands.CreateComment;

public interface ICreateCommentHandler
{
    Task<Result<CommentDto>> Handle(CreateCommentCommand command, CancellationToken ct);
}

public class CreateCommentHandler : ICreateCommentHandler
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;
    private readonly IActivityLogger _activity;
    private readonly INotificationDispatcher _notifications;

    public CreateCommentHandler(IApplicationDbContext db, ICurrentUserService currentUser,
        IActivityLogger activity, INotificationDispatcher notifications)
    {
        _db = db;
        _currentUser = currentUser;
        _activity = activity;
        _notifications = notifications;
    }

    public async Task<Result<CommentDto>> Handle(CreateCommentCommand command, CancellationToken ct)
    {
        var task = await _db.Tasks.AsNoTracking()
            .Where(t => t.Id == command.TaskId)
            .Select(t => new { t.OrganizationId, t.TaskKey, t.Title, t.AssigneeUserId })
            .FirstOrDefaultAsync(ct);
        if (task is null) return Result<CommentDto>.Fail("Task not found.");

        if (command.ParentCommentId is not null)
        {
            var parent = await _db.Comments.AsNoTracking()
                .Where(c => c.Id == command.ParentCommentId)
                .Select(c => new { c.TaskId, c.ParentCommentId })
                .FirstOrDefaultAsync(ct);
            if (parent is null || parent.TaskId != command.TaskId)
            {
                return Result<CommentDto>.FailValidation(new[] { new ValidationError(nameof(command.ParentCommentId), "Parent comment not found on this task.") });
            }
            if (parent.ParentCommentId is not null)
            {
                return Result<CommentDto>.FailValidation(new[] { new ValidationError(nameof(command.ParentCommentId), "Comments can only be nested one level deep.") });
            }
        }

        var mentionIds = (command.MentionedUserIds ?? Array.Empty<Guid>()).Distinct().ToList();
        if (mentionIds.Count > 0)
        {
            var validCount = await _db.OrganizationUsers.AsNoTracking()
                .CountAsync(ou => ou.OrganizationId == task.OrganizationId && mentionIds.Contains(ou.UserId), ct);
            if (validCount != mentionIds.Count)
            {
                return Result<CommentDto>.FailValidation(new[] { new ValidationError(nameof(command.MentionedUserIds), "One or more mentioned users are not members of this organization.") });
            }
        }

        var comment = new Comment
        {
            OrganizationId = task.OrganizationId,
            TaskId = command.TaskId,
            AuthorUserId = _currentUser.UserId!.Value,
            Body = command.Body.Trim(),
            ParentCommentId = command.ParentCommentId
        };
        foreach (var userId in mentionIds)
        {
            comment.Mentions.Add(new CommentMention { UserId = userId });
        }
        _db.Comments.Add(comment);
        await _db.SaveChangesAsync(ct);

        await _activity.LogAsync("Task", command.TaskId, "CommentAdded", null, new { comment.Id }, ct);

        // Mentioned users get a direct mention notification; watchers + assignee get the softer
        // "new comment" notification. Anyone in both sets is deduped to just the mention (mentions
        // dispatched first, and the acting user is excluded by the dispatcher).
        if (mentionIds.Count > 0)
        {
            await _notifications.DispatchAsync(mentionIds, NotificationType.CommentMention,
                $"You were mentioned on {task.TaskKey}", task.Title, "Task", command.TaskId, ct);
        }

        var watcherRecipients = await _db.TaskWatchers.AsNoTracking()
            .Where(w => w.TaskId == command.TaskId).Select(w => w.UserId).ToListAsync(ct);
        if (task.AssigneeUserId is not null) watcherRecipients.Add(task.AssigneeUserId.Value);
        var commentAddedRecipients = watcherRecipients.Except(mentionIds).ToList();
        if (commentAddedRecipients.Count > 0)
        {
            await _notifications.DispatchAsync(commentAddedRecipients, NotificationType.CommentAdded,
                $"New comment on {task.TaskKey}", task.Title, "Task", command.TaskId, ct);
        }

        var dto = await _db.Comments.AsNoTracking()
            .Where(c => c.Id == comment.Id)
            .Select(CommentProjection.ToDto)
            .FirstAsync(ct);
        return Result<CommentDto>.Ok(dto);
    }
}
