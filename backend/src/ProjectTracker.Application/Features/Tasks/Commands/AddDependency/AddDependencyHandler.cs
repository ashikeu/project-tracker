using Microsoft.EntityFrameworkCore;
using ProjectTracker.Application.Common.Interfaces;
using ProjectTracker.Domain.Entities;
using ProjectTracker.Shared.Results;

namespace ProjectTracker.Application.Features.Tasks.Commands.AddDependency;

public interface IAddDependencyHandler
{
    Task<Result> Handle(AddDependencyCommand command, CancellationToken ct);
}

/// <summary>Cycle prevention requires walking the existing dependency graph, which needs
/// persistence access — so unlike most Domain business rules, this one lives here rather than
/// in Domain/BusinessRules (see CLAUDE.md "Validation split" for the same exception applied to
/// uniqueness checks).</summary>
public class AddDependencyHandler : IAddDependencyHandler
{
    private readonly IApplicationDbContext _db;

    public AddDependencyHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result> Handle(AddDependencyCommand command, CancellationToken ct)
    {
        if (command.TaskId == command.DependsOnTaskId)
        {
            return Result.Fail("A task cannot depend on itself.");
        }

        var taskExists = await _db.Tasks.AsNoTracking().AnyAsync(t => t.Id == command.TaskId, ct);
        var dependsOnExists = await _db.Tasks.AsNoTracking().AnyAsync(t => t.Id == command.DependsOnTaskId, ct);
        if (!taskExists || !dependsOnExists) return Result.Fail("One or both tasks were not found.");

        var alreadyExists = await _db.TaskDependencies.AsNoTracking()
            .AnyAsync(d => d.TaskId == command.TaskId && d.DependsOnTaskId == command.DependsOnTaskId && d.Type == command.Type, ct);
        if (alreadyExists) return Result.Ok("Dependency already exists.");

        if (await CreatesCycleAsync(command.TaskId, command.DependsOnTaskId, command.Type, ct))
        {
            return Result.Fail("This would create a circular dependency between tasks.");
        }

        _db.TaskDependencies.Add(new TaskDependency { TaskId = command.TaskId, DependsOnTaskId = command.DependsOnTaskId, Type = command.Type });
        await _db.SaveChangesAsync(ct);
        return Result.Ok("Dependency added.");
    }

    /// <summary>True if `dependsOnTaskId` already (directly or transitively) depends on
    /// `taskId` for the same dependency Type — adding the requested edge would close a cycle.</summary>
    private async Task<bool> CreatesCycleAsync(Guid taskId, Guid dependsOnTaskId, Domain.Enums.TaskDependencyType type, CancellationToken ct)
    {
        var visited = new HashSet<Guid>();
        var queue = new Queue<Guid>();
        queue.Enqueue(dependsOnTaskId);

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            if (current == taskId) return true;
            if (!visited.Add(current)) continue;

            var next = await _db.TaskDependencies.AsNoTracking()
                .Where(d => d.TaskId == current && d.Type == type)
                .Select(d => d.DependsOnTaskId)
                .ToListAsync(ct);
            foreach (var n in next) queue.Enqueue(n);
        }

        return false;
    }
}
