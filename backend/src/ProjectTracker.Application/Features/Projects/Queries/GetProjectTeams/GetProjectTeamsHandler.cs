using Microsoft.EntityFrameworkCore;
using ProjectTracker.Application.Common.Interfaces;
using ProjectTracker.Application.Features.Projects.Dtos;

namespace ProjectTracker.Application.Features.Projects.Queries.GetProjectTeams;

public record GetProjectTeamsQuery(Guid ProjectId);

public interface IGetProjectTeamsHandler
{
    Task<IReadOnlyList<ProjectTeamDto>> Handle(GetProjectTeamsQuery query, CancellationToken ct);
}

public class GetProjectTeamsHandler : IGetProjectTeamsHandler
{
    private readonly IApplicationDbContext _db;

    public GetProjectTeamsHandler(IApplicationDbContext db) => _db = db;

    public async Task<IReadOnlyList<ProjectTeamDto>> Handle(GetProjectTeamsQuery query, CancellationToken ct)
    {
        return await _db.ProjectTeams.AsNoTracking()
            .Where(pt => pt.ProjectId == query.ProjectId)
            .Select(pt => new ProjectTeamDto(pt.TeamId, pt.Team.Name))
            .ToListAsync(ct);
    }
}
