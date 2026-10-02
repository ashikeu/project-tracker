using Microsoft.EntityFrameworkCore;
using ProjectTracker.Application.Common.Interfaces;
using ProjectTracker.Application.Features.Boards.Dtos;
using ProjectTracker.Domain.Entities;
using ProjectTracker.Shared.Results;

namespace ProjectTracker.Application.Features.Boards.Commands.CreateBoardColumn;

public interface ICreateBoardColumnHandler
{
    Task<Result<BoardColumnDto>> Handle(CreateBoardColumnCommand command, CancellationToken ct);
}

public class CreateBoardColumnHandler : ICreateBoardColumnHandler
{
    private readonly IApplicationDbContext _db;

    public CreateBoardColumnHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result<BoardColumnDto>> Handle(CreateBoardColumnCommand command, CancellationToken ct)
    {
        var projectExists = await _db.Projects.AsNoTracking().AnyAsync(p => p.Id == command.ProjectId, ct);
        if (!projectExists) return Result<BoardColumnDto>.Fail("Project not found.");

        var maxOrder = await _db.BoardColumns.AsNoTracking()
            .Where(c => c.ProjectId == command.ProjectId).Select(c => (int?)c.Order).MaxAsync(ct) ?? -1;

        var column = new BoardColumn
        {
            ProjectId = command.ProjectId,
            Name = command.Name.Trim(),
            Color = command.Color,
            Order = maxOrder + 1,
            WipLimit = command.WipLimit,
            StatusMapping = command.StatusMapping,
            IsCompletionColumn = command.IsCompletionColumn
        };
        _db.BoardColumns.Add(column);
        await _db.SaveChangesAsync(ct);

        return Result<BoardColumnDto>.Ok(new BoardColumnDto(
            column.Id, column.ProjectId, column.Name, column.Color, column.Order, column.WipLimit,
            column.StatusMapping.ToString(), column.IsCompletionColumn, 0));
    }
}
