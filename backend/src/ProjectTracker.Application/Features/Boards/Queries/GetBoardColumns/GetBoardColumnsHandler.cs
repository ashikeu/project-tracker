using Microsoft.EntityFrameworkCore;
using ProjectTracker.Application.Common.Interfaces;
using ProjectTracker.Application.Features.Boards.Dtos;

namespace ProjectTracker.Application.Features.Boards.Queries.GetBoardColumns;

public record GetBoardColumnsQuery(Guid ProjectId);

public interface IGetBoardColumnsHandler
{
    Task<IReadOnlyList<BoardColumnDto>> Handle(GetBoardColumnsQuery query, CancellationToken ct);
}

public class GetBoardColumnsHandler : IGetBoardColumnsHandler
{
    private readonly IApplicationDbContext _db;

    public GetBoardColumnsHandler(IApplicationDbContext db) => _db = db;

    public async Task<IReadOnlyList<BoardColumnDto>> Handle(GetBoardColumnsQuery query, CancellationToken ct)
    {
        return await _db.BoardColumns.AsNoTracking()
            .Where(c => c.ProjectId == query.ProjectId)
            .OrderBy(c => c.Order)
            .Select(c => new BoardColumnDto(
                c.Id, c.ProjectId, c.Name, c.Color, c.Order, c.WipLimit, c.StatusMapping.ToString(),
                c.IsCompletionColumn, _db.Tasks.Count(t => t.BoardColumnId == c.Id)))
            .ToListAsync(ct);
    }
}
