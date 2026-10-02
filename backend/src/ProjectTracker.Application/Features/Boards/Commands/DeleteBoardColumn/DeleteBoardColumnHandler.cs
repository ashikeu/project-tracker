using Microsoft.EntityFrameworkCore;
using ProjectTracker.Application.Common.Interfaces;
using ProjectTracker.Shared.Results;

namespace ProjectTracker.Application.Features.Boards.Commands.DeleteBoardColumn;

public record DeleteBoardColumnCommand(Guid ColumnId);

public interface IDeleteBoardColumnHandler
{
    Task<Result> Handle(DeleteBoardColumnCommand command, CancellationToken ct);
}

public class DeleteBoardColumnHandler : IDeleteBoardColumnHandler
{
    private readonly IApplicationDbContext _db;

    public DeleteBoardColumnHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result> Handle(DeleteBoardColumnCommand command, CancellationToken ct)
    {
        var column = await _db.BoardColumns.FirstOrDefaultAsync(c => c.Id == command.ColumnId, ct);
        if (column is null) return Result.Fail("Board column not found.");

        var hasTasks = await _db.Tasks.AsNoTracking().AnyAsync(t => t.BoardColumnId == command.ColumnId, ct);
        if (hasTasks)
        {
            return Result.Fail("This column still has tasks in it. Move them to another column before deleting it.");
        }

        _db.BoardColumns.Remove(column);
        await _db.SaveChangesAsync(ct);
        return Result.Ok("Board column deleted.");
    }
}
