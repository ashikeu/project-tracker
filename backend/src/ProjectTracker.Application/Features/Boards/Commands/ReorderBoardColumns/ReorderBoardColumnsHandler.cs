using Microsoft.EntityFrameworkCore;
using ProjectTracker.Application.Common.Interfaces;
using ProjectTracker.Shared.Results;

namespace ProjectTracker.Application.Features.Boards.Commands.ReorderBoardColumns;

public record ReorderBoardColumnsCommand(Guid ProjectId, IReadOnlyList<Guid> OrderedColumnIds);

public interface IReorderBoardColumnsHandler
{
    Task<Result> Handle(ReorderBoardColumnsCommand command, CancellationToken ct);
}

public class ReorderBoardColumnsHandler : IReorderBoardColumnsHandler
{
    private readonly IApplicationDbContext _db;

    public ReorderBoardColumnsHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result> Handle(ReorderBoardColumnsCommand command, CancellationToken ct)
    {
        var columns = await _db.BoardColumns.Where(c => c.ProjectId == command.ProjectId).ToListAsync(ct);
        var columnsById = columns.ToDictionary(c => c.Id);

        if (command.OrderedColumnIds.Count != columns.Count || command.OrderedColumnIds.Any(id => !columnsById.ContainsKey(id)))
        {
            return Result.Fail("The provided column list must include every column of this project exactly once.");
        }

        for (var i = 0; i < command.OrderedColumnIds.Count; i++)
        {
            columnsById[command.OrderedColumnIds[i]].Order = i;
        }

        await _db.SaveChangesAsync(ct);
        return Result.Ok("Columns reordered.");
    }
}
