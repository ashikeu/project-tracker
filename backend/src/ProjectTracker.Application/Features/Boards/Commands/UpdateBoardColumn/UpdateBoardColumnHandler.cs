using Microsoft.EntityFrameworkCore;
using ProjectTracker.Application.Common.Interfaces;
using ProjectTracker.Application.Features.Boards.Dtos;
using ProjectTracker.Shared.Results;

namespace ProjectTracker.Application.Features.Boards.Commands.UpdateBoardColumn;

public interface IUpdateBoardColumnHandler
{
    Task<Result<BoardColumnDto>> Handle(UpdateBoardColumnCommand command, CancellationToken ct);
}

public class UpdateBoardColumnHandler : IUpdateBoardColumnHandler
{
    private readonly IApplicationDbContext _db;

    public UpdateBoardColumnHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result<BoardColumnDto>> Handle(UpdateBoardColumnCommand command, CancellationToken ct)
    {
        var column = await _db.BoardColumns.FirstOrDefaultAsync(c => c.Id == command.ColumnId, ct);
        if (column is null) return Result<BoardColumnDto>.Fail("Board column not found.");

        column.Name = command.Name.Trim();
        column.Color = command.Color;
        column.WipLimit = command.WipLimit;
        column.StatusMapping = command.StatusMapping;
        column.IsCompletionColumn = command.IsCompletionColumn;
        await _db.SaveChangesAsync(ct);

        var taskCount = await _db.Tasks.AsNoTracking().CountAsync(t => t.BoardColumnId == column.Id, ct);
        return Result<BoardColumnDto>.Ok(new BoardColumnDto(
            column.Id, column.ProjectId, column.Name, column.Color, column.Order, column.WipLimit,
            column.StatusMapping.ToString(), column.IsCompletionColumn, taskCount));
    }
}
