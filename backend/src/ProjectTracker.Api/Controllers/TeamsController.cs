using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectTracker.Api.Common;
using ProjectTracker.Application.Common.Interfaces;
using ProjectTracker.Application.Features.Teams.Commands.AddTeamMember;
using ProjectTracker.Application.Features.Teams.Commands.ArchiveTeam;
using ProjectTracker.Application.Features.Teams.Commands.CreateTeam;
using ProjectTracker.Application.Features.Teams.Commands.DeleteTeam;
using ProjectTracker.Application.Features.Teams.Commands.RemoveTeamMember;
using ProjectTracker.Application.Features.Teams.Commands.UpdateTeam;
using ProjectTracker.Application.Features.Teams.Commands.UpdateTeamMemberRole;
using ProjectTracker.Application.Features.Teams.Queries.GetTeam;
using ProjectTracker.Application.Features.Teams.Queries.GetTeams;
using ProjectTracker.Domain.Enums;
using ProjectTracker.Shared.Constants;

namespace ProjectTracker.Api.Controllers;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/teams")]
[Authorize]
public class TeamsController : ApiControllerBase
{
    private readonly ICurrentUserService _currentUser;

    public TeamsController(ICurrentUserService currentUser) => _currentUser = currentUser;

    [HttpGet]
    [Authorize(Policy = PermissionKeys.TeamView)]
    public async Task<IActionResult> GetTeams([FromQuery] bool includeArchived, [FromQuery] int page, [FromQuery] int pageSize, [FromServices] IGetTeamsHandler handler, CancellationToken ct)
    {
        if (_currentUser.OrganizationId is null) return Unauthorized();
        var result = await handler.Handle(new GetTeamsQuery(_currentUser.OrganizationId.Value, includeArchived, page, pageSize), ct);
        return Ok(ApiResponse<object>.Ok(result));
    }

    [HttpGet("{id:guid}")]
    [Authorize(Policy = PermissionKeys.TeamView)]
    public async Task<IActionResult> GetTeam(Guid id, [FromServices] IGetTeamHandler handler, CancellationToken ct)
    {
        var result = await handler.Handle(new GetTeamQuery(id), ct);
        return FromResult(result);
    }

    [HttpPost]
    [Authorize(Policy = PermissionKeys.TeamCreate)]
    public async Task<IActionResult> CreateTeam([FromBody] CreateTeamRequest request, [FromServices] ICreateTeamHandler handler, CancellationToken ct)
    {
        if (_currentUser.OrganizationId is null || _currentUser.UserId is null) return Unauthorized();
        var result = await handler.Handle(new CreateTeamCommand(_currentUser.OrganizationId.Value, _currentUser.UserId.Value, request.Name, request.Description, request.LeadUserId), ct);
        return FromResult(result);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = PermissionKeys.TeamUpdate)]
    public async Task<IActionResult> UpdateTeam(Guid id, [FromBody] UpdateTeamRequest request, [FromServices] IUpdateTeamHandler handler, CancellationToken ct)
    {
        var result = await handler.Handle(new UpdateTeamCommand(id, request.Name, request.Description, request.LeadUserId), ct);
        return FromResult(result);
    }

    [HttpPost("{id:guid}/archive")]
    [Authorize(Policy = PermissionKeys.TeamUpdate)]
    public async Task<IActionResult> Archive(Guid id, [FromQuery] bool archive, [FromServices] IArchiveTeamHandler handler, CancellationToken ct)
    {
        var result = await handler.Handle(new ArchiveTeamCommand(id, archive), ct);
        return FromResult(result);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = PermissionKeys.TeamDelete)]
    public async Task<IActionResult> Delete(Guid id, [FromServices] IDeleteTeamHandler handler, CancellationToken ct)
    {
        var result = await handler.Handle(new DeleteTeamCommand(id), ct);
        return FromResult(result);
    }

    [HttpPost("{id:guid}/members")]
    [Authorize(Policy = PermissionKeys.TeamManageMembers)]
    public async Task<IActionResult> AddMember(Guid id, [FromBody] AddTeamMemberRequest request, [FromServices] IAddTeamMemberHandler handler, CancellationToken ct)
    {
        var result = await handler.Handle(new AddTeamMemberCommand(id, request.UserId, request.Role), ct);
        return FromResult(result);
    }

    [HttpPut("{id:guid}/members/{memberId:guid}")]
    [Authorize(Policy = PermissionKeys.TeamManageMembers)]
    public async Task<IActionResult> UpdateMemberRole(Guid id, Guid memberId, [FromBody] UpdateTeamMemberRoleRequest request, [FromServices] IUpdateTeamMemberRoleHandler handler, CancellationToken ct)
    {
        var result = await handler.Handle(new UpdateTeamMemberRoleCommand(id, memberId, request.Role), ct);
        return FromResult(result);
    }

    [HttpDelete("{id:guid}/members/{memberId:guid}")]
    [Authorize(Policy = PermissionKeys.TeamManageMembers)]
    public async Task<IActionResult> RemoveMember(Guid id, Guid memberId, [FromServices] IRemoveTeamMemberHandler handler, CancellationToken ct)
    {
        var result = await handler.Handle(new RemoveTeamMemberCommand(id, memberId), ct);
        return FromResult(result);
    }
}

public record CreateTeamRequest(string Name, string? Description, Guid? LeadUserId);
public record UpdateTeamRequest(string Name, string? Description, Guid? LeadUserId);
public record AddTeamMemberRequest(Guid UserId, TeamMemberRole Role);
public record UpdateTeamMemberRoleRequest(TeamMemberRole Role);
