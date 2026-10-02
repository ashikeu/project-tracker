using Microsoft.EntityFrameworkCore;
using ProjectTracker.Application.Common.Interfaces;
using ProjectTracker.Application.Features.Projects.Dtos;
using ProjectTracker.Shared.Results;

namespace ProjectTracker.Application.Features.Projects.Queries.GetProject;

public record GetProjectQuery(Guid ProjectId);

public interface IGetProjectHandler
{
    Task<Result<ProjectDto>> Handle(GetProjectQuery query, CancellationToken ct);
}

public class GetProjectHandler : IGetProjectHandler
{
    private readonly IApplicationDbContext _db;

    public GetProjectHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result<ProjectDto>> Handle(GetProjectQuery query, CancellationToken ct)
    {
        var project = await _db.Projects.AsNoTracking()
            .Where(p => p.Id == query.ProjectId)
            .Select(p => new ProjectDto(
                p.Id, p.Key, p.Name, p.Description,
                p.OwnerUserId, p.Owner.FirstName + " " + p.Owner.LastName,
                p.ManagerUserId, p.Manager != null ? p.Manager.FirstName + " " + p.Manager.LastName : null,
                p.Status.ToString(), p.Priority.ToString(), p.StartDate, p.TargetDate,
                p.IsArchived, p.CreatedAt))
            .FirstOrDefaultAsync(ct);

        return project is null ? Result<ProjectDto>.Fail("Project not found.") : Result<ProjectDto>.Ok(project);
    }
}
