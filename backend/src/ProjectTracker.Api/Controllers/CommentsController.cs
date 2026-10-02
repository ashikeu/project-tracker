using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectTracker.Api.Common;
using ProjectTracker.Application.Features.Comments.Commands.CreateComment;
using ProjectTracker.Application.Features.Comments.Commands.DeleteComment;
using ProjectTracker.Application.Features.Comments.Commands.UpdateComment;
using ProjectTracker.Application.Features.Comments.Queries.GetComments;
using ProjectTracker.Shared.Constants;

namespace ProjectTracker.Api.Controllers;

[ApiVersion("1.0")]
[Authorize]
public class CommentsController : ApiControllerBase
{
    [HttpGet("api/v{version:apiVersion}/tasks/{taskId:guid}/comments")]
    [Authorize(Policy = PermissionKeys.TaskView)]
    public async Task<IActionResult> GetComments(Guid taskId, [FromQuery] int page, [FromQuery] int pageSize, [FromServices] IGetCommentsHandler handler, CancellationToken ct)
    {
        var result = await handler.Handle(new GetCommentsQuery(taskId, page <= 0 ? 1 : page, pageSize), ct);
        return Ok(ApiResponse<object>.Ok(result));
    }

    [HttpPost("api/v{version:apiVersion}/tasks/{taskId:guid}/comments")]
    [Authorize(Policy = PermissionKeys.CommentCreate)]
    public async Task<IActionResult> CreateComment(Guid taskId, [FromBody] CreateCommentRequest request, [FromServices] ICreateCommentHandler handler, CancellationToken ct)
    {
        var result = await handler.Handle(new CreateCommentCommand(taskId, request.Body, request.ParentCommentId, request.MentionedUserIds), ct);
        return FromResult(result);
    }

    [HttpPut("api/v{version:apiVersion}/comments/{id:guid}")]
    [Authorize(Policy = PermissionKeys.CommentCreate)]
    public async Task<IActionResult> UpdateComment(Guid id, [FromBody] UpdateCommentRequest request, [FromServices] IUpdateCommentHandler handler, CancellationToken ct)
    {
        var result = await handler.Handle(new UpdateCommentCommand(id, request.Body, request.MentionedUserIds), ct);
        return FromResult(result);
    }

    [HttpDelete("api/v{version:apiVersion}/comments/{id:guid}")]
    [Authorize(Policy = PermissionKeys.CommentCreate)]
    public async Task<IActionResult> DeleteComment(Guid id, [FromServices] IDeleteCommentHandler handler, CancellationToken ct)
    {
        var result = await handler.Handle(new DeleteCommentCommand(id), ct);
        return FromResult(result);
    }
}

public record CreateCommentRequest(string Body, Guid? ParentCommentId, IReadOnlyList<Guid>? MentionedUserIds);
public record UpdateCommentRequest(string Body, IReadOnlyList<Guid>? MentionedUserIds);
