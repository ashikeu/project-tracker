using Microsoft.EntityFrameworkCore;
using ProjectTracker.Application.Common.Interfaces;
using ProjectTracker.Application.Features.Teams.Dtos;
using ProjectTracker.Shared.Results;

namespace ProjectTracker.Application.Features.Teams.Commands.UpdateTeam;

public interface IUpdateTeamHandler
{
    Task<Result<TeamDto>> Handle(UpdateTeamCommand command, CancellationToken ct);
}

public class UpdateTeamHandler : IUpdateTeamHandler
{
    private readonly IApplicationDbContext _db;

    public UpdateTeamHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result<TeamDto>> Handle(UpdateTeamCommand command, CancellationToken ct)
    {
        var team = await _db.Teams.FirstOrDefaultAsync(t => t.Id == command.TeamId, ct);
        if (team is null) return Result<TeamDto>.Fail("Team not found.");

        if (command.LeadUserId is not null)
        {
            var leadExists = await _db.OrganizationUsers.AsNoTracking()
                .AnyAsync(ou => ou.OrganizationId == team.OrganizationId && ou.UserId == command.LeadUserId, ct);
            if (!leadExists)
            {
                return Result<TeamDto>.FailValidation(new[]
                {
                    new Shared.Results.ValidationError(nameof(command.LeadUserId), "The specified lead is not a member of this organization.")
                });
            }
        }

        team.Name = command.Name.Trim();
        team.Description = command.Description?.Trim();
        team.LeadUserId = command.LeadUserId;

        await _db.SaveChangesAsync(ct);

        var memberCount = await _db.TeamMembers.AsNoTracking().CountAsync(tm => tm.TeamId == team.Id, ct);
        var leadName = command.LeadUserId is null
            ? null
            : await _db.Users.AsNoTracking().Where(u => u.Id == command.LeadUserId).Select(u => u.FirstName + " " + u.LastName).FirstOrDefaultAsync(ct);

        return Result<TeamDto>.Ok(new TeamDto(team.Id, team.Name, team.Description, team.LeadUserId, leadName, team.IsArchived, memberCount, team.CreatedAt));
    }
}
