using Microsoft.EntityFrameworkCore;
using ProjectTracker.Application.Common.Interfaces;
using ProjectTracker.Shared.Results;

namespace ProjectTracker.Application.Features.Tasks.Commands.UpdateChecklistItem;

public record UpdateChecklistItemCommand(Guid ChecklistItemId, string Text, bool IsDone, int Order);

public interface IUpdateChecklistItemHandler
{
    Task<Result> Handle(UpdateChecklistItemCommand command, CancellationToken ct);
}

public class UpdateChecklistItemHandler : IUpdateChecklistItemHandler
{
    private readonly IApplicationDbContext _db;

    public UpdateChecklistItemHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result> Handle(UpdateChecklistItemCommand command, CancellationToken ct)
    {
        var item = await _db.ChecklistItems.FirstOrDefaultAsync(c => c.Id == command.ChecklistItemId, ct);
        if (item is null) return Result.Fail("Checklist item not found.");

        item.Text = command.Text.Trim();
        item.IsDone = command.IsDone;
        item.Order = command.Order;
        await _db.SaveChangesAsync(ct);

        return Result.Ok("Checklist item updated.");
    }
}
