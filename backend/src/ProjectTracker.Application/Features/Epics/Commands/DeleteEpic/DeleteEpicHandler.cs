using Microsoft.EntityFrameworkCore;
using ProjectTracker.Application.Common.Interfaces;
using ProjectTracker.Shared.Results;

namespace ProjectTracker.Application.Features.Epics.Commands.DeleteEpic;

public record DeleteEpicCommand(Guid EpicId);

public interface IDeleteEpicHandler
{
    Task<Result> Handle(DeleteEpicCommand command, CancellationToken ct);
}

public class DeleteEpicHandler : IDeleteEpicHandler
{
    private readonly IApplicationDbContext _db;

    public DeleteEpicHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result> Handle(DeleteEpicCommand command, CancellationToken ct)
    {
        var epic = await _db.Epics.FirstOrDefaultAsync(e => e.Id == command.EpicId, ct);
        if (epic is null) return Result.Fail("Epic not found.");

        var hasTasks = await _db.Tasks.AsNoTracking().AnyAsync(t => t.EpicId == command.EpicId, ct);
        if (hasTasks)
        {
            return Result.Fail("This epic still has tasks assigned to it. Reassign or remove them before deleting the epic.");
        }

        epic.IsDeleted = true;
        await _db.SaveChangesAsync(ct);
        return Result.Ok("Epic deleted.");
    }
}
