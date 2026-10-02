using Microsoft.EntityFrameworkCore;
using ProjectTracker.Application.Common.Interfaces;
using ProjectTracker.Shared.Results;

namespace ProjectTracker.Application.Features.Projects.Commands.DeleteProject;

public record DeleteProjectCommand(Guid ProjectId);

public interface IDeleteProjectHandler
{
    Task<Result> Handle(DeleteProjectCommand command, CancellationToken ct);
}

public class DeleteProjectHandler : IDeleteProjectHandler
{
    private readonly IApplicationDbContext _db;

    public DeleteProjectHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result> Handle(DeleteProjectCommand command, CancellationToken ct)
    {
        var project = await _db.Projects.FirstOrDefaultAsync(p => p.Id == command.ProjectId, ct);
        if (project is null) return Result.Fail("Project not found.");

        project.IsDeleted = true; // ApplicationDbContext.SaveChangesAsync stamps DeletedAt/DeletedBy
        await _db.SaveChangesAsync(ct);
        return Result.Ok("Project deleted.");
    }
}
