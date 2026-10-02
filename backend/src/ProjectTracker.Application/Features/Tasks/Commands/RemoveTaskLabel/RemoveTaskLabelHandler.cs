using Microsoft.EntityFrameworkCore;
using ProjectTracker.Application.Common.Interfaces;
using ProjectTracker.Shared.Results;

namespace ProjectTracker.Application.Features.Tasks.Commands.RemoveTaskLabel;

public record RemoveTaskLabelCommand(Guid TaskId, Guid LabelId);

public interface IRemoveTaskLabelHandler
{
    Task<Result> Handle(RemoveTaskLabelCommand command, CancellationToken ct);
}

public class RemoveTaskLabelHandler : IRemoveTaskLabelHandler
{
    private readonly IApplicationDbContext _db;

    public RemoveTaskLabelHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result> Handle(RemoveTaskLabelCommand command, CancellationToken ct)
    {
        var link = await _db.TaskLabels.FirstOrDefaultAsync(tl => tl.TaskId == command.TaskId && tl.LabelId == command.LabelId, ct);
        if (link is null) return Result.Ok("Label not applied.");

        _db.TaskLabels.Remove(link);
        await _db.SaveChangesAsync(ct);
        return Result.Ok("Label removed.");
    }
}
