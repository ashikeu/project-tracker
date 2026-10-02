using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectTracker.Api.Common;
using ProjectTracker.Application.Common.Interfaces;
using ProjectTracker.Application.Features.Projects.Commands.AddProjectMember;
using ProjectTracker.Application.Features.Projects.Commands.AddProjectTeam;
using ProjectTracker.Application.Features.Projects.Commands.ArchiveProject;
using ProjectTracker.Application.Features.Projects.Commands.CreateProject;
using ProjectTracker.Application.Features.Projects.Commands.DeleteProject;
using ProjectTracker.Application.Features.Projects.Commands.RemoveProjectMember;
using ProjectTracker.Application.Features.Projects.Commands.RemoveProjectTeam;
using ProjectTracker.Application.Features.Projects.Commands.UpdateProject;
using ProjectTracker.Application.Features.Projects.Queries.GetProject;
using ProjectTracker.Application.Features.Projects.Queries.GetProjectMembers;
using ProjectTracker.Application.Features.Projects.Queries.GetProjects;
using ProjectTracker.Application.Features.Projects.Queries.GetProjectTeams;
using ProjectTracker.Application.Features.Tasks.Commands.CreateLabel;
using ProjectTracker.Application.Features.Tasks.Commands.DeleteLabel;
using ProjectTracker.Application.Features.Tasks.Queries.GetLabels;
using ProjectTracker.Domain.Enums;
using ProjectTracker.Shared.Constants;

namespace ProjectTracker.Api.Controllers;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/projects")]
[Authorize]
public class ProjectsController : ApiControllerBase
{
    private readonly ICurrentUserService _currentUser;

    public ProjectsController(ICurrentUserService currentUser) => _currentUser = currentUser;

    [HttpGet]
    [Authorize(Policy = PermissionKeys.ProjectView)]
    public async Task<IActionResult> GetProjects(
        [FromQuery] bool includeArchived, [FromQuery] ProjectStatus? status, [FromQuery] string? search,
        [FromQuery] int page, [FromQuery] int pageSize, [FromServices] IGetProjectsHandler handler, CancellationToken ct)
    {
        if (_currentUser.OrganizationId is null) return Unauthorized();
        var result = await handler.Handle(new GetProjectsQuery(_currentUser.OrganizationId.Value, includeArchived, status, search, page, pageSize), ct);
        return Ok(ApiResponse<object>.Ok(result));
    }

    [HttpGet("{id:guid}")]
    [Authorize(Policy = PermissionKeys.ProjectView)]
    public async Task<IActionResult> GetProject(Guid id, [FromServices] IGetProjectHandler handler, CancellationToken ct)
    {
        var result = await handler.Handle(new GetProjectQuery(id), ct);
        return FromResult(result);
    }

    [HttpPost]
    [Authorize(Policy = PermissionKeys.ProjectCreate)]
    public async Task<IActionResult> CreateProject([FromBody] CreateProjectRequest request, [FromServices] ICreateProjectHandler handler, CancellationToken ct)
    {
        if (_currentUser.OrganizationId is null || _currentUser.UserId is null) return Unauthorized();
        var result = await handler.Handle(new CreateProjectCommand(
            _currentUser.OrganizationId.Value, _currentUser.UserId.Value, request.Key, request.Name, request.Description,
            request.ManagerUserId, request.Priority, request.StartDate, request.TargetDate), ct);
        return FromResult(result);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = PermissionKeys.ProjectUpdate)]
    public async Task<IActionResult> UpdateProject(Guid id, [FromBody] UpdateProjectRequest request, [FromServices] IUpdateProjectHandler handler, CancellationToken ct)
    {
        var result = await handler.Handle(new UpdateProjectCommand(
            id, request.Name, request.Description, request.ManagerUserId, request.Status, request.Priority, request.StartDate, request.TargetDate), ct);
        return FromResult(result);
    }

    [HttpPost("{id:guid}/archive")]
    [Authorize(Policy = PermissionKeys.ProjectUpdate)]
    public async Task<IActionResult> Archive(Guid id, [FromQuery] bool archive, [FromServices] IArchiveProjectHandler handler, CancellationToken ct)
    {
        var result = await handler.Handle(new ArchiveProjectCommand(id, archive), ct);
        return FromResult(result);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = PermissionKeys.ProjectDelete)]
    public async Task<IActionResult> Delete(Guid id, [FromServices] IDeleteProjectHandler handler, CancellationToken ct)
    {
        var result = await handler.Handle(new DeleteProjectCommand(id), ct);
        return FromResult(result);
    }

    [HttpGet("{id:guid}/members")]
    [Authorize(Policy = PermissionKeys.ProjectView)]
    public async Task<IActionResult> GetMembers(Guid id, [FromServices] IGetProjectMembersHandler handler, CancellationToken ct)
    {
        var result = await handler.Handle(new GetProjectMembersQuery(id), ct);
        return Ok(ApiResponse<object>.Ok(result));
    }

    [HttpPost("{id:guid}/members")]
    [Authorize(Policy = PermissionKeys.ProjectManageMembers)]
    public async Task<IActionResult> AddMember(Guid id, [FromBody] AddProjectMemberRequest request, [FromServices] IAddProjectMemberHandler handler, CancellationToken ct)
    {
        var result = await handler.Handle(new AddProjectMemberCommand(id, request.UserId, request.RoleId), ct);
        return FromResult(result);
    }

    [HttpDelete("{id:guid}/members/{memberId:guid}")]
    [Authorize(Policy = PermissionKeys.ProjectManageMembers)]
    public async Task<IActionResult> RemoveMember(Guid id, Guid memberId, [FromServices] IRemoveProjectMemberHandler handler, CancellationToken ct)
    {
        var result = await handler.Handle(new RemoveProjectMemberCommand(id, memberId), ct);
        return FromResult(result);
    }

    [HttpGet("{id:guid}/teams")]
    [Authorize(Policy = PermissionKeys.ProjectView)]
    public async Task<IActionResult> GetTeams(Guid id, [FromServices] IGetProjectTeamsHandler handler, CancellationToken ct)
    {
        var result = await handler.Handle(new GetProjectTeamsQuery(id), ct);
        return Ok(ApiResponse<object>.Ok(result));
    }

    [HttpPost("{id:guid}/teams")]
    [Authorize(Policy = PermissionKeys.ProjectManageMembers)]
    public async Task<IActionResult> AddTeam(Guid id, [FromBody] AddProjectTeamRequest request, [FromServices] IAddProjectTeamHandler handler, CancellationToken ct)
    {
        var result = await handler.Handle(new AddProjectTeamCommand(id, request.TeamId), ct);
        return FromResult(result);
    }

    [HttpDelete("{id:guid}/teams/{teamId:guid}")]
    [Authorize(Policy = PermissionKeys.ProjectManageMembers)]
    public async Task<IActionResult> RemoveTeam(Guid id, Guid teamId, [FromServices] IRemoveProjectTeamHandler handler, CancellationToken ct)
    {
        var result = await handler.Handle(new RemoveProjectTeamCommand(id, teamId), ct);
        return FromResult(result);
    }

    [HttpGet("{id:guid}/labels")]
    [Authorize(Policy = PermissionKeys.ProjectView)]
    public async Task<IActionResult> GetLabels(Guid id, [FromServices] IGetLabelsHandler handler, CancellationToken ct)
    {
        var result = await handler.Handle(new GetLabelsQuery(id), ct);
        return Ok(ApiResponse<object>.Ok(result));
    }

    [HttpPost("{id:guid}/labels")]
    [Authorize(Policy = PermissionKeys.ProjectUpdate)]
    public async Task<IActionResult> CreateLabel(Guid id, [FromBody] CreateLabelRequest request, [FromServices] ICreateLabelHandler handler, CancellationToken ct)
    {
        var result = await handler.Handle(new CreateLabelCommand(id, request.Name, request.Color), ct);
        return FromResult(result);
    }

    [HttpDelete("labels/{labelId:guid}")]
    [Authorize(Policy = PermissionKeys.ProjectUpdate)]
    public async Task<IActionResult> DeleteLabel(Guid labelId, [FromServices] IDeleteLabelHandler handler, CancellationToken ct)
    {
        var result = await handler.Handle(new DeleteLabelCommand(labelId), ct);
        return FromResult(result);
    }
}

public record CreateProjectRequest(string Key, string Name, string? Description, Guid? ManagerUserId, Priority Priority, DateOnly? StartDate, DateOnly? TargetDate);
public record UpdateProjectRequest(string Name, string? Description, Guid? ManagerUserId, ProjectStatus Status, Priority Priority, DateOnly? StartDate, DateOnly? TargetDate);
public record AddProjectMemberRequest(Guid UserId, Guid RoleId);
public record AddProjectTeamRequest(Guid TeamId);
public record CreateLabelRequest(string Name, string Color);
