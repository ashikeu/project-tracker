using Microsoft.EntityFrameworkCore;
using ProjectTracker.Application.Common.Interfaces;
using ProjectTracker.Shared.Results;

namespace ProjectTracker.Application.Features.Tasks.Commands.DeleteLabel;

public record DeleteLabelCommand(Guid LabelId);

public interface IDeleteLabelHandler
{
    Task<Result> Handle(DeleteLabelCommand command, CancellationToken ct);
}

public class DeleteLabelHandler : IDeleteLabelHandler
{
    private readonly IApplicationDbContext _db;

    public DeleteLabelHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result> Handle(DeleteLabelCommand command, CancellationToken ct)
    {
        var label = await _db.Labels.FirstOrDefaultAsync(l => l.Id == command.LabelId, ct);
        if (label is null) return Result.Fail("Label not found.");

        _db.Labels.Remove(label);
        await _db.SaveChangesAsync(ct);
        return Result.Ok("Label deleted.");
    }
}
