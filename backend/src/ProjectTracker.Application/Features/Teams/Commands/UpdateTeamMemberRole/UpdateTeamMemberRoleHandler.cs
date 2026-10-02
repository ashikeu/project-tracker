using Microsoft.EntityFrameworkCore;
using ProjectTracker.Application.Common.Interfaces;
using ProjectTracker.Domain.Enums;
using ProjectTracker.Shared.Results;

namespace ProjectTracker.Application.Features.Teams.Commands.UpdateTeamMemberRole;

public record UpdateTeamMemberRoleCommand(Guid TeamId, Guid TeamMemberId, TeamMemberRole Role);

public interface IUpdateTeamMemberRoleHandler
{
    Task<Result> Handle(UpdateTeamMemberRoleCommand command, CancellationToken ct);
}

public class UpdateTeamMemberRoleHandler : IUpdateTeamMemberRoleHandler
{
    private readonly IApplicationDbContext _db;

    public UpdateTeamMemberRoleHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result> Handle(UpdateTeamMemberRoleCommand command, CancellationToken ct)
    {
        var member = await _db.TeamMembers.FirstOrDefaultAsync(tm => tm.Id == command.TeamMemberId && tm.TeamId == command.TeamId, ct);
        if (member is null) return Result.Fail("Team member not found.");

        if (member.Role == TeamMemberRole.Owner && command.Role != TeamMemberRole.Owner)
        {
            var otherOwners = await _db.TeamMembers.AsNoTracking()
                .CountAsync(tm => tm.TeamId == command.TeamId && tm.Role == TeamMemberRole.Owner && tm.Id != member.Id, ct);
            if (otherOwners == 0)
            {
                return Result.Fail("A team must always have at least one Owner.");
            }
        }

        member.Role = command.Role;
        await _db.SaveChangesAsync(ct);
        return Result.Ok("Team member role updated.");
    }
}
