using Microsoft.EntityFrameworkCore;
using ProjectTracker.Application.Common.Interfaces;
using ProjectTracker.Application.Features.SavedFilters.Dtos;

namespace ProjectTracker.Application.Features.SavedFilters.Queries.GetSavedFilters;

public record GetSavedFiltersQuery(string? Scope, Guid? ProjectId);

public interface IGetSavedFiltersHandler
{
    Task<IReadOnlyList<SavedFilterDto>> Handle(GetSavedFiltersQuery query, CancellationToken ct);
}

/// <summary>The current user's saved filters, optionally narrowed by scope and/or project.</summary>
public class GetSavedFiltersHandler : IGetSavedFiltersHandler
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public GetSavedFiltersHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<IReadOnlyList<SavedFilterDto>> Handle(GetSavedFiltersQuery query, CancellationToken ct)
    {
        var userId = _currentUser.UserId;
        var q = _db.SavedFilters.AsNoTracking().Where(f => f.UserId == userId);
        if (!string.IsNullOrWhiteSpace(query.Scope)) q = q.Where(f => f.Scope == query.Scope);
        if (query.ProjectId is not null) q = q.Where(f => f.ProjectId == query.ProjectId);

        return await q.OrderBy(f => f.Name)
            .Select(f => new SavedFilterDto(f.Id, f.ProjectId, f.Name, f.Scope, f.FilterJson, f.CreatedAt))
            .ToListAsync(ct);
    }
}
