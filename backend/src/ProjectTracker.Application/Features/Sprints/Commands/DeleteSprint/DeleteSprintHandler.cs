using Microsoft.EntityFrameworkCore;
using ProjectTracker.Application.Common.Interfaces;
using ProjectTracker.Shared.Results;

namespace ProjectTracker.Application.Features.Sprints.Commands.DeleteSprint;

public record DeleteSprintCommand(Guid SprintId);

public interface IDeleteSprintHandler
{
    Task<Result> Handle(DeleteSprintCommand command, CancellationToken ct);
}

public class DeleteSprintHandler : IDeleteSprintHandler
{
    private readonly IApplicationDbContext _db;

    public DeleteSprintHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result> Handle(DeleteSprintCommand command, CancellationToken ct)
    {
        var sprint = await _db.Sprints.FirstOrDefaultAsync(s => s.Id == command.SprintId, ct);
        if (sprint is null) return Result.Fail("Sprint not found.");

        var hasTasks = await _db.Tasks.AsNoTracking().AnyAsync(t => t.SprintId == command.SprintId, ct);
        if (hasTasks)
        {
            return Result.Fail("This sprint still has tasks assigned to it. Move them to the backlog or another sprint before deleting.");
        }

        sprint.IsDeleted = true;
        await _db.SaveChangesAsync(ct);
        return Result.Ok("Sprint deleted.");
    }
}
