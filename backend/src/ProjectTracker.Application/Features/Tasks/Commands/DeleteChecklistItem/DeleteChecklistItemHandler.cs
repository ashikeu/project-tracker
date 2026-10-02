using Microsoft.EntityFrameworkCore;
using ProjectTracker.Application.Common.Interfaces;
using ProjectTracker.Shared.Results;

namespace ProjectTracker.Application.Features.Tasks.Commands.DeleteChecklistItem;

public record DeleteChecklistItemCommand(Guid ChecklistItemId);

public interface IDeleteChecklistItemHandler
{
    Task<Result> Handle(DeleteChecklistItemCommand command, CancellationToken ct);
}

public class DeleteChecklistItemHandler : IDeleteChecklistItemHandler
{
    private readonly IApplicationDbContext _db;

    public DeleteChecklistItemHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result> Handle(DeleteChecklistItemCommand command, CancellationToken ct)
    {
        var item = await _db.ChecklistItems.FirstOrDefaultAsync(c => c.Id == command.ChecklistItemId, ct);
        if (item is null) return Result.Fail("Checklist item not found.");

        _db.ChecklistItems.Remove(item);
        await _db.SaveChangesAsync(ct);
        return Result.Ok("Checklist item deleted.");
    }
}
