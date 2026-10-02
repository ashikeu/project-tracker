using Microsoft.EntityFrameworkCore;
using ProjectTracker.Application.Common.Interfaces;
using ProjectTracker.Application.Features.Teams.Dtos;
using ProjectTracker.Domain.Entities;
using ProjectTracker.Domain.Enums;
using ProjectTracker.Shared.Results;

namespace ProjectTracker.Application.Features.Teams.Commands.CreateTeam;

public interface ICreateTeamHandler
{
    Task<Result<TeamDto>> Handle(CreateTeamCommand command, CancellationToken ct);
}

public class CreateTeamHandler : ICreateTeamHandler
{
    private readonly IApplicationDbContext _db;

    public CreateTeamHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result<TeamDto>> Handle(CreateTeamCommand command, CancellationToken ct)
    {
        if (command.LeadUserId is not null)
        {
            var leadExists = await _db.OrganizationUsers.AsNoTracking()
                .AnyAsync(ou => ou.OrganizationId == command.OrganizationId && ou.UserId == command.LeadUserId, ct);
            if (!leadExists)
            {
                return Result<TeamDto>.FailValidation(new[]
                {
                    new ValidationError(nameof(command.LeadUserId), "The specified lead is not a member of this organization.")
                });
            }
        }

        var team = new Team
        {
            OrganizationId = command.OrganizationId,
            Name = command.Name.Trim(),
            Description = command.Description?.Trim(),
            LeadUserId = command.LeadUserId
        };
        _db.Teams.Add(team);

        // The creator becomes Team Owner by default.
        _db.TeamMembers.Add(new TeamMember
        {
            Team = team,
            UserId = command.CreatedByUserId,
            Role = TeamMemberRole.Owner
        });

        await _db.SaveChangesAsync(ct);

        var leadName = command.LeadUserId is null
            ? null
            : await _db.Users.AsNoTracking().Where(u => u.Id == command.LeadUserId).Select(u => u.FirstName + " " + u.LastName).FirstOrDefaultAsync(ct);

        return Result<TeamDto>.Ok(new TeamDto(team.Id, team.Name, team.Description, team.LeadUserId, leadName, team.IsArchived, 1, team.CreatedAt));
    }
}
