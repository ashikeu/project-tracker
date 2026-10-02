using Microsoft.EntityFrameworkCore;
using ProjectTracker.Application.Common.Interfaces;
using ProjectTracker.Application.Features.Teams.Dtos;
using ProjectTracker.Shared.Results;

namespace ProjectTracker.Application.Features.Teams.Queries.GetTeam;

public record GetTeamQuery(Guid TeamId);

public record TeamDetailDto(TeamDto Team, IReadOnlyList<TeamMemberDto> Members);

public interface IGetTeamHandler
{
    Task<Result<TeamDetailDto>> Handle(GetTeamQuery query, CancellationToken ct);
}

public class GetTeamHandler : IGetTeamHandler
{
    private readonly IApplicationDbContext _db;

    public GetTeamHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result<TeamDetailDto>> Handle(GetTeamQuery query, CancellationToken ct)
    {
        var team = await _db.Teams.AsNoTracking()
            .Where(t => t.Id == query.TeamId)
            .Select(t => new TeamDto(
                t.Id, t.Name, t.Description, t.LeadUserId,
                t.Lead != null ? t.Lead.FirstName + " " + t.Lead.LastName : null,
                t.IsArchived, t.Members.Count, t.CreatedAt))
            .FirstOrDefaultAsync(ct);

        if (team is null) return Result<TeamDetailDto>.Fail("Team not found.");

        var members = await _db.TeamMembers.AsNoTracking()
            .Where(tm => tm.TeamId == query.TeamId)
            .OrderBy(tm => tm.User.FirstName)
            .Select(tm => new TeamMemberDto(tm.Id, tm.UserId, tm.User.Email, tm.User.FirstName, tm.User.LastName, tm.User.AvatarUrl, tm.Role.ToString()))
            .ToListAsync(ct);

        return Result<TeamDetailDto>.Ok(new TeamDetailDto(team, members));
    }
}
