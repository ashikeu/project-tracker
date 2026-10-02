using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectTracker.Api.Common;
using ProjectTracker.Application.Features.Attachments.Commands.DeleteAttachment;
using ProjectTracker.Application.Features.Attachments.Commands.UploadAttachment;
using ProjectTracker.Application.Features.Attachments.Queries.DownloadAttachment;
using ProjectTracker.Application.Features.Attachments.Queries.GetAttachments;
using ProjectTracker.Domain.Enums;
using ProjectTracker.Shared.Constants;

namespace ProjectTracker.Api.Controllers;

[ApiVersion("1.0")]
[Authorize]
public class AttachmentsController : ApiControllerBase
{
    [HttpGet("api/v{version:apiVersion}/tasks/{taskId:guid}/attachments")]
    [Authorize(Policy = PermissionKeys.TaskView)]
    public async Task<IActionResult> GetTaskAttachments(Guid taskId, [FromQuery] int page, [FromQuery] int pageSize, [FromServices] IGetAttachmentsHandler handler, CancellationToken ct)
    {
        var result = await handler.Handle(new GetAttachmentsQuery(AttachmentEntityType.Task, taskId, page <= 0 ? 1 : page, pageSize), ct);
        return Ok(ApiResponse<object>.Ok(result));
    }

    [HttpPost("api/v{version:apiVersion}/tasks/{taskId:guid}/attachments")]
    [Authorize(Policy = PermissionKeys.AttachmentUpload)]
    public async Task<IActionResult> UploadTaskAttachment(Guid taskId, IFormFile file, [FromServices] IUploadAttachmentHandler handler, CancellationToken ct)
    {
        if (file is null || file.Length == 0)
        {
            return BadRequest(new ApiResponse { Success = false, Message = "No file was uploaded." });
        }

        await using var stream = file.OpenReadStream();
        var result = await handler.Handle(new UploadAttachmentCommand(
            AttachmentEntityType.Task, taskId, stream, file.FileName, file.ContentType, file.Length), ct);
        return FromResult(result);
    }

    [HttpGet("api/v{version:apiVersion}/attachments/{id:guid}/download")]
    [Authorize(Policy = PermissionKeys.TaskView)]
    public async Task<IActionResult> Download(Guid id, [FromServices] IDownloadAttachmentHandler handler, CancellationToken ct)
    {
        var result = await handler.Handle(new DownloadAttachmentQuery(id), ct);
        if (!result.Success || result.Data is null) return FromResult(result);
        return File(result.Data.Content, result.Data.ContentType, result.Data.FileName);
    }

    [HttpDelete("api/v{version:apiVersion}/attachments/{id:guid}")]
    [Authorize(Policy = PermissionKeys.AttachmentDelete)]
    public async Task<IActionResult> Delete(Guid id, [FromServices] IDeleteAttachmentHandler handler, CancellationToken ct)
    {
        var result = await handler.Handle(new DeleteAttachmentCommand(id), ct);
        return FromResult(result);
    }
}
