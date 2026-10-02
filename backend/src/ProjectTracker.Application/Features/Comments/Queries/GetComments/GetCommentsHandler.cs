using Microsoft.EntityFrameworkCore;
using ProjectTracker.Application.Common.Interfaces;
using ProjectTracker.Application.Features.Comments.Dtos;
using ProjectTracker.Application.Features.Comments.Mappings;
using ProjectTracker.Shared.Results;

namespace ProjectTracker.Application.Features.Comments.Queries.GetComments;

public record GetCommentsQuery(Guid TaskId, int Page, int PageSize);

public interface IGetCommentsHandler
{
    Task<PagedResult<CommentDto>> Handle(GetCommentsQuery query, CancellationToken ct);
}

/// <summary>Returns top-level comments (newest first) with their replies nested; pagination is over
/// the top-level threads, so a thread and all its replies always travel together.</summary>
public class GetCommentsHandler : IGetCommentsHandler
{
    private readonly IApplicationDbContext _db;

    public GetCommentsHandler(IApplicationDbContext db) => _db = db;

    public async Task<PagedResult<CommentDto>> Handle(GetCommentsQuery query, CancellationToken ct)
    {
        var q = _db.Comments.AsNoTracking()
            .Where(c => c.TaskId == query.TaskId && c.ParentCommentId == null)
            .OrderByDescending(c => c.CreatedAt);

        var totalCount = await q.CountAsync(ct);
        var page = Math.Max(query.Page, 1);
        var pageSize = query.PageSize <= 0 ? 50 : Math.Min(query.PageSize, 200);

        var items = await q.Skip((page - 1) * pageSize).Take(pageSize)
            .Select(CommentProjection.ToDto).ToListAsync(ct);

        return new PagedResult<CommentDto>(items, page, pageSize, totalCount);
    }
}
