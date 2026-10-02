using Microsoft.EntityFrameworkCore;
using ProjectTracker.Application.Common.Interfaces;
using ProjectTracker.Application.Features.Projects.Dtos;

namespace ProjectTracker.Application.Features.Projects.Queries.GetProjectMembers;

public record GetProjectMembersQuery(Guid ProjectId);

public interface IGetProjectMembersHandler
{
    Task<IReadOnlyList<ProjectMemberDto>> Handle(GetProjectMembersQuery query, CancellationToken ct);
}

public class GetProjectMembersHandler : IGetProjectMembersHandler
{
    private readonly IApplicationDbContext _db;

    public GetProjectMembersHandler(IApplicationDbContext db) => _db = db;

    public async Task<IReadOnlyList<ProjectMemberDto>> Handle(GetProjectMembersQuery query, CancellationToken ct)
    {
        return await _db.ProjectMembers.AsNoTracking()
            .Where(pm => pm.ProjectId == query.ProjectId)
            .OrderBy(pm => pm.User.FirstName)
            .Select(pm => new ProjectMemberDto(pm.Id, pm.UserId, pm.User.Email, pm.User.FirstName, pm.User.LastName, pm.User.AvatarUrl, pm.RoleId, pm.Role.Name))
            .ToListAsync(ct);
    }
}
