using Microsoft.EntityFrameworkCore;
using ProjectTracker.Application.Common.Interfaces;
using ProjectTracker.Application.Features.Tasks.Dtos;
using ProjectTracker.Domain.Entities;
using ProjectTracker.Shared.Results;

namespace ProjectTracker.Application.Features.Tasks.Commands.AddChecklistItem;

public record AddChecklistItemCommand(Guid TaskId, string Text);

public interface IAddChecklistItemHandler
{
    Task<Result<ChecklistItemDto>> Handle(AddChecklistItemCommand command, CancellationToken ct);
}

public class AddChecklistItemHandler : IAddChecklistItemHandler
{
    private readonly IApplicationDbContext _db;

    public AddChecklistItemHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result<ChecklistItemDto>> Handle(AddChecklistItemCommand command, CancellationToken ct)
    {
        var taskExists = await _db.Tasks.AsNoTracking().AnyAsync(t => t.Id == command.TaskId, ct);
        if (!taskExists) return Result<ChecklistItemDto>.Fail("Task not found.");

        var nextOrder = await _db.ChecklistItems.AsNoTracking()
            .Where(c => c.TaskId == command.TaskId).Select(c => (int?)c.Order).MaxAsync(ct) ?? -1;

        var item = new ChecklistItem { TaskId = command.TaskId, Text = command.Text.Trim(), Order = nextOrder + 1 };
        _db.ChecklistItems.Add(item);
        await _db.SaveChangesAsync(ct);

        return Result<ChecklistItemDto>.Ok(new ChecklistItemDto(item.Id, item.Text, item.IsDone, item.Order));
    }
}
