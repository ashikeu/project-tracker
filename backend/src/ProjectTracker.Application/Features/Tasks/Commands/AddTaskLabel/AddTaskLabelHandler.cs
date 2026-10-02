using Microsoft.EntityFrameworkCore;
using ProjectTracker.Application.Common.Interfaces;
using ProjectTracker.Domain.Entities;
using ProjectTracker.Shared.Results;

namespace ProjectTracker.Application.Features.Tasks.Commands.AddTaskLabel;

public record AddTaskLabelCommand(Guid TaskId, Guid LabelId);

public interface IAddTaskLabelHandler
{
    Task<Result> Handle(AddTaskLabelCommand command, CancellationToken ct);
}

public class AddTaskLabelHandler : IAddTaskLabelHandler
{
    private readonly IApplicationDbContext _db;

    public AddTaskLabelHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result> Handle(AddTaskLabelCommand command, CancellationToken ct)
    {
        var task = await _db.Tasks.AsNoTracking().FirstOrDefaultAsync(t => t.Id == command.TaskId, ct);
        if (task is null) return Result.Fail("Task not found.");

        var labelValid = await _db.Labels.AsNoTracking().AnyAsync(l => l.Id == command.LabelId && l.ProjectId == task.ProjectId, ct);
        if (!labelValid) return Result.Fail("Label not found in this project.");

        var alreadyApplied = await _db.TaskLabels.AsNoTracking().AnyAsync(tl => tl.TaskId == command.TaskId && tl.LabelId == command.LabelId, ct);
        if (alreadyApplied) return Result.Ok("Label already applied.");

        _db.TaskLabels.Add(new TaskLabel { TaskId = command.TaskId, LabelId = command.LabelId });
        await _db.SaveChangesAsync(ct);
        return Result.Ok("Label applied.");
    }
}
