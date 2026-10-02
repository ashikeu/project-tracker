using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectTracker.Api.Common;
using ProjectTracker.Application.Features.Notifications.Commands.MarkAllRead;
using ProjectTracker.Application.Features.Notifications.Commands.MarkNotificationRead;
using ProjectTracker.Application.Features.Notifications.Commands.UpdatePreference;
using ProjectTracker.Application.Features.Notifications.Queries.GetNotifications;
using ProjectTracker.Application.Features.Notifications.Queries.GetPreferences;
using ProjectTracker.Application.Features.Notifications.Queries.GetUnreadCount;
using ProjectTracker.Domain.Enums;

namespace ProjectTracker.Api.Controllers;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/notifications")]
[Authorize]
public class NotificationsController : ApiControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetNotifications([FromQuery] bool unreadOnly, [FromQuery] int page, [FromQuery] int pageSize, [FromServices] IGetNotificationsHandler handler, CancellationToken ct)
    {
        var result = await handler.Handle(new GetNotificationsQuery(unreadOnly, page <= 0 ? 1 : page, pageSize), ct);
        return Ok(ApiResponse<object>.Ok(result));
    }

    [HttpGet("unread-count")]
    public async Task<IActionResult> GetUnreadCount([FromServices] IGetUnreadCountHandler handler, CancellationToken ct)
    {
        var count = await handler.Handle(new GetUnreadCountQuery(), ct);
        return Ok(ApiResponse<object>.Ok(new { unreadCount = count }));
    }

    [HttpPost("{id:guid}/read")]
    public async Task<IActionResult> MarkRead(Guid id, [FromServices] IMarkNotificationReadHandler handler, CancellationToken ct)
    {
        var result = await handler.Handle(new MarkNotificationReadCommand(id), ct);
        return FromResult(result);
    }

    [HttpPost("read-all")]
    public async Task<IActionResult> MarkAllRead([FromServices] IMarkAllReadHandler handler, CancellationToken ct)
    {
        var result = await handler.Handle(new MarkAllReadCommand(), ct);
        return FromResult(result);
    }

    [HttpGet("preferences")]
    public async Task<IActionResult> GetPreferences([FromServices] IGetPreferencesHandler handler, CancellationToken ct)
    {
        var result = await handler.Handle(new GetPreferencesQuery(), ct);
        return Ok(ApiResponse<object>.Ok(result));
    }

    [HttpPut("preferences")]
    public async Task<IActionResult> UpdatePreference([FromBody] UpdatePreferenceRequest request, [FromServices] IUpdatePreferenceHandler handler, CancellationToken ct)
    {
        var result = await handler.Handle(new UpdatePreferenceCommand(request.NotificationType, request.InAppEnabled, request.PushEnabled), ct);
        return FromResult(result);
    }
}

public record UpdatePreferenceRequest(NotificationType NotificationType, bool InAppEnabled, bool PushEnabled);
