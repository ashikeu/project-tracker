using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectTracker.Api.Common;
using ProjectTracker.Application.Common.Interfaces;
using ProjectTracker.Application.Features.Organizations.Commands.CreateInvitation;
using ProjectTracker.Application.Features.Organizations.Commands.UpdateOrganization;
using ProjectTracker.Application.Features.Organizations.Queries.GetOrganization;
using ProjectTracker.Application.Features.Organizations.Queries.GetOrganizationMembers;
using ProjectTracker.Application.Features.Organizations.Queries.GetOrganizationRoles;
using ProjectTracker.Shared.Constants;

namespace ProjectTracker.Api.Controllers;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/organizations")]
[Authorize]
public class OrganizationsController : ApiControllerBase
{
    private readonly ICurrentUserService _currentUser;

    public OrganizationsController(ICurrentUserService currentUser)
    {
        _currentUser = currentUser;
    }

    /// <summary>Only the caller's own organization can be accessed — the route id is checked
    /// against the JWT's org claim rather than trusted outright, so no client-supplied id can
    /// read another tenant's data.</summary>
    private bool IsCallersOrganization(Guid organizationId) => _currentUser.OrganizationId == organizationId;

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id, [FromServices] IGetOrganizationHandler handler, CancellationToken ct)
    {
        if (!IsCallersOrganization(id)) return Forbid();
        var result = await handler.Handle(new GetOrganizationQuery(id), ct);
        return FromResult(result);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = PermissionKeys.OrganizationManage)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateOrganizationRequest request, [FromServices] IUpdateOrganizationHandler handler, CancellationToken ct)
    {
        if (!IsCallersOrganization(id)) return Forbid();
        var result = await handler.Handle(new UpdateOrganizationCommand(id, request.Name), ct);
        return FromResult(result);
    }

    [HttpGet("{id:guid}/members")]
    [Authorize(Policy = PermissionKeys.OrganizationViewMembers)]
    public async Task<IActionResult> GetMembers(Guid id, [FromQuery] int page, [FromQuery] int pageSize, [FromServices] IGetOrganizationMembersHandler handler, CancellationToken ct)
    {
        if (!IsCallersOrganization(id)) return Forbid();
        var result = await handler.Handle(new GetOrganizationMembersQuery(id, page, pageSize), ct);
        return Ok(ProjectTracker.Api.Common.ApiResponse<object>.Ok(result));
    }

    [HttpPost("{id:guid}/invitations")]
    [Authorize(Policy = PermissionKeys.OrganizationInviteMembers)]
    public async Task<IActionResult> CreateInvitation(Guid id, [FromBody] CreateInvitationRequest request, [FromServices] ICreateInvitationHandler handler, CancellationToken ct)
    {
        if (!IsCallersOrganization(id)) return Forbid();
        if (_currentUser.UserId is null) return Unauthorized();
        var result = await handler.Handle(new CreateInvitationCommand(id, _currentUser.UserId.Value, request.Email, request.RoleId), ct);
        return FromResult(result);
    }

    [HttpGet("{id:guid}/roles")]
    public async Task<IActionResult> GetRoles(Guid id, [FromServices] IGetOrganizationRolesHandler handler, CancellationToken ct)
    {
        if (!IsCallersOrganization(id)) return Forbid();
        var result = await handler.Handle(new GetOrganizationRolesQuery(id), ct);
        return Ok(ProjectTracker.Api.Common.ApiResponse<object>.Ok(result));
    }
}

public record UpdateOrganizationRequest(string Name);
public record CreateInvitationRequest(string Email, Guid RoleId);
