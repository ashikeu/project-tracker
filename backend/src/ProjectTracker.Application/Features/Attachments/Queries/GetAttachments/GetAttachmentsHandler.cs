using Microsoft.EntityFrameworkCore;
using ProjectTracker.Application.Common.Interfaces;
using ProjectTracker.Application.Features.Attachments.Dtos;
using ProjectTracker.Domain.Enums;
using ProjectTracker.Shared.Results;

namespace ProjectTracker.Application.Features.Attachments.Queries.GetAttachments;

public record GetAttachmentsQuery(AttachmentEntityType EntityType, Guid EntityId, int Page, int PageSize);

public interface IGetAttachmentsHandler
{
    Task<PagedResult<AttachmentDto>> Handle(GetAttachmentsQuery query, CancellationToken ct);
}

public class GetAttachmentsHandler : IGetAttachmentsHandler
{
    private readonly IApplicationDbContext _db;

    public GetAttachmentsHandler(IApplicationDbContext db) => _db = db;

    public async Task<PagedResult<AttachmentDto>> Handle(GetAttachmentsQuery query, CancellationToken ct)
    {
        var q = _db.Attachments.AsNoTracking()
            .Where(a => a.EntityType == query.EntityType && a.EntityId == query.EntityId)
            .OrderByDescending(a => a.UploadedAt);

        var totalCount = await q.CountAsync(ct);
        var page = Math.Max(query.Page, 1);
        var pageSize = query.PageSize <= 0 ? 50 : Math.Min(query.PageSize, 200);

        var items = await q.Skip((page - 1) * pageSize).Take(pageSize)
            .Select(a => new AttachmentDto(
                a.Id, a.EntityType.ToString(), a.EntityId, a.OriginalFileName, a.ContentType,
                a.FileSize, a.UploadedByUserId, a.UploadedBy.FirstName + " " + a.UploadedBy.LastName, a.UploadedAt))
            .ToListAsync(ct);

        return new PagedResult<AttachmentDto>(items, page, pageSize, totalCount);
    }
}
