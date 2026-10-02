using Microsoft.EntityFrameworkCore;
using ProjectTracker.Application.Common.Interfaces;
using ProjectTracker.Domain.Entities;
using ProjectTracker.Shared.Results;

namespace ProjectTracker.Application.Features.Projects.Commands.AddProjectTeam;

public record AddProjectTeamCommand(Guid ProjectId, Guid TeamId);

public interface IAddProjectTeamHandler
{
    Task<Result> Handle(AddProjectTeamCommand command, CancellationToken ct);
}

public class AddProjectTeamHandler : IAddProjectTeamHandler
{
    private readonly IApplicationDbContext _db;

    public AddProjectTeamHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result> Handle(AddProjectTeamCommand command, CancellationToken ct)
    {
        var project = await _db.Projects.AsNoTracking().FirstOrDefaultAsync(p => p.Id == command.ProjectId, ct);
        if (project is null) return Result.Fail("Project not found.");

        var team = await _db.Teams.AsNoTracking().FirstOrDefaultAsync(t => t.Id == command.TeamId, ct);
        if (team is null || team.OrganizationId != project.OrganizationId)
        {
            return Result.Fail("Team not found in this organization.");
        }

        var alreadyLinked = await _db.ProjectTeams.AsNoTracking()
            .AnyAsync(pt => pt.ProjectId == command.ProjectId && pt.TeamId == command.TeamId, ct);
        if (alreadyLinked) return Result.Fail("This team is already assigned to the project.");

        _db.ProjectTeams.Add(new ProjectTeam { ProjectId = command.ProjectId, TeamId = command.TeamId });
        await _db.SaveChangesAsync(ct);
        return Result.Ok("Team added to project.");
    }
}
