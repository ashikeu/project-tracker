using Microsoft.EntityFrameworkCore;
using ProjectTracker.Application.Common.Interfaces;
using ProjectTracker.Shared.Results;

namespace ProjectTracker.Application.Features.Projects.Commands.RemoveProjectTeam;

public record RemoveProjectTeamCommand(Guid ProjectId, Guid TeamId);

public interface IRemoveProjectTeamHandler
{
    Task<Result> Handle(RemoveProjectTeamCommand command, CancellationToken ct);
}

public class RemoveProjectTeamHandler : IRemoveProjectTeamHandler
{
    private readonly IApplicationDbContext _db;

    public RemoveProjectTeamHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result> Handle(RemoveProjectTeamCommand command, CancellationToken ct)
    {
        var link = await _db.ProjectTeams.FirstOrDefaultAsync(pt => pt.ProjectId == command.ProjectId && pt.TeamId == command.TeamId, ct);
        if (link is null) return Result.Fail("This team is not assigned to the project.");

        _db.ProjectTeams.Remove(link);
        await _db.SaveChangesAsync(ct);
        return Result.Ok("Team removed from project.");
    }
}
