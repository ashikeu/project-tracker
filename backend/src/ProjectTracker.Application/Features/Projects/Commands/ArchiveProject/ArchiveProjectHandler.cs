using Microsoft.EntityFrameworkCore;
using ProjectTracker.Application.Common.Interfaces;
using ProjectTracker.Shared.Results;

namespace ProjectTracker.Application.Features.Projects.Commands.ArchiveProject;

public record ArchiveProjectCommand(Guid ProjectId, bool Archive);

public interface IArchiveProjectHandler
{
    Task<Result> Handle(ArchiveProjectCommand command, CancellationToken ct);
}

public class ArchiveProjectHandler : IArchiveProjectHandler
{
    private readonly IApplicationDbContext _db;

    public ArchiveProjectHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result> Handle(ArchiveProjectCommand command, CancellationToken ct)
    {
        var project = await _db.Projects.FirstOrDefaultAsync(p => p.Id == command.ProjectId, ct);
        if (project is null) return Result.Fail("Project not found.");

        project.IsArchived = command.Archive;
        project.ArchivedAt = command.Archive ? DateTime.UtcNow : null;
        await _db.SaveChangesAsync(ct);

        return Result.Ok(command.Archive ? "Project archived." : "Project restored.");
    }
}
