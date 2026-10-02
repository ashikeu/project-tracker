using Microsoft.EntityFrameworkCore;
using ProjectTracker.Application.Common.Interfaces;
using ProjectTracker.Shared.Results;

namespace ProjectTracker.Application.Features.Teams.Commands.DeleteTeam;

public record DeleteTeamCommand(Guid TeamId);

public interface IDeleteTeamHandler
{
    Task<Result> Handle(DeleteTeamCommand command, CancellationToken ct);
}

public class DeleteTeamHandler : IDeleteTeamHandler
{
    private readonly IApplicationDbContext _db;

    public DeleteTeamHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result> Handle(DeleteTeamCommand command, CancellationToken ct)
    {
        var team = await _db.Teams.FirstOrDefaultAsync(t => t.Id == command.TeamId, ct);
        if (team is null) return Result.Fail("Team not found.");

        team.IsDeleted = true; // ApplicationDbContext.SaveChangesAsync stamps DeletedAt/DeletedBy
        await _db.SaveChangesAsync(ct);
        return Result.Ok("Team deleted.");
    }
}
