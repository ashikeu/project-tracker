using Microsoft.EntityFrameworkCore;
using ProjectTracker.Application.Common.Interfaces;
using ProjectTracker.Shared.Results;

namespace ProjectTracker.Application.Features.Teams.Commands.ArchiveTeam;

public record ArchiveTeamCommand(Guid TeamId, bool Archive);

public interface IArchiveTeamHandler
{
    Task<Result> Handle(ArchiveTeamCommand command, CancellationToken ct);
}

/// <summary>Toggles the team's Archived status — a reversible "hide from active views" flag,
/// distinct from Delete (soft-delete, see DeleteTeamHandler).</summary>
public class ArchiveTeamHandler : IArchiveTeamHandler
{
    private readonly IApplicationDbContext _db;

    public ArchiveTeamHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result> Handle(ArchiveTeamCommand command, CancellationToken ct)
    {
        var team = await _db.Teams.FirstOrDefaultAsync(t => t.Id == command.TeamId, ct);
        if (team is null) return Result.Fail("Team not found.");

        team.IsArchived = command.Archive;
        await _db.SaveChangesAsync(ct);
        return Result.Ok(command.Archive ? "Team archived." : "Team restored.");
    }
}
