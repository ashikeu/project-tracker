using Microsoft.EntityFrameworkCore;
using ProjectTracker.Application.Common.Interfaces;
using ProjectTracker.Shared.Results;

namespace ProjectTracker.Application.Features.Tasks.Commands.RemoveDependency;

public record RemoveDependencyCommand(Guid DependencyId);

public interface IRemoveDependencyHandler
{
    Task<Result> Handle(RemoveDependencyCommand command, CancellationToken ct);
}

public class RemoveDependencyHandler : IRemoveDependencyHandler
{
    private readonly IApplicationDbContext _db;

    public RemoveDependencyHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result> Handle(RemoveDependencyCommand command, CancellationToken ct)
    {
        var dependency = await _db.TaskDependencies.FirstOrDefaultAsync(d => d.Id == command.DependencyId, ct);
        if (dependency is null) return Result.Fail("Dependency not found.");

        _db.TaskDependencies.Remove(dependency);
        await _db.SaveChangesAsync(ct);
        return Result.Ok("Dependency removed.");
    }
}
