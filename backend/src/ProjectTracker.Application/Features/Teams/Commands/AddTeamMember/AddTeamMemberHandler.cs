using Microsoft.EntityFrameworkCore;
using ProjectTracker.Application.Common.Interfaces;
using ProjectTracker.Application.Features.Teams.Dtos;
using ProjectTracker.Domain.Entities;
using ProjectTracker.Shared.Results;

namespace ProjectTracker.Application.Features.Teams.Commands.AddTeamMember;

public interface IAddTeamMemberHandler
{
    Task<Result<TeamMemberDto>> Handle(AddTeamMemberCommand command, CancellationToken ct);
}

public class AddTeamMemberHandler : IAddTeamMemberHandler
{
    private readonly IApplicationDbContext _db;

    public AddTeamMemberHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result<TeamMemberDto>> Handle(AddTeamMemberCommand command, CancellationToken ct)
    {
        var team = await _db.Teams.AsNoTracking().FirstOrDefaultAsync(t => t.Id == command.TeamId, ct);
        if (team is null) return Result<TeamMemberDto>.Fail("Team not found.");

        var isOrgMember = await _db.OrganizationUsers.AsNoTracking()
            .AnyAsync(ou => ou.OrganizationId == team.OrganizationId && ou.UserId == command.UserId, ct);
        if (!isOrgMember)
        {
            return Result<TeamMemberDto>.FailValidation(new[]
            {
                new ValidationError(nameof(command.UserId), "This user is not a member of the organization.")
            });
        }

        var alreadyMember = await _db.TeamMembers.AsNoTracking()
            .AnyAsync(tm => tm.TeamId == command.TeamId && tm.UserId == command.UserId, ct);
        if (alreadyMember)
        {
            return Result<TeamMemberDto>.Fail("This user is already a member of the team.");
        }

        var member = new TeamMember { TeamId = command.TeamId, UserId = command.UserId, Role = command.Role };
        _db.TeamMembers.Add(member);
        await _db.SaveChangesAsync(ct);

        var user = await _db.Users.AsNoTracking().FirstAsync(u => u.Id == command.UserId, ct);
        return Result<TeamMemberDto>.Ok(new TeamMemberDto(member.Id, user.Id, user.Email, user.FirstName, user.LastName, user.AvatarUrl, member.Role.ToString()));
    }
}
