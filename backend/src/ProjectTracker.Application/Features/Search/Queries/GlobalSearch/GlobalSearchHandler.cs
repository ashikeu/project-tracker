using Microsoft.EntityFrameworkCore;
using ProjectTracker.Application.Common.Interfaces;
using ProjectTracker.Application.Features.Search.Dtos;
using ProjectTracker.Shared.Results;

namespace ProjectTracker.Application.Features.Search.Queries.GlobalSearch;

/// <summary>Types is the optional set to include ("projects","epics","tasks","users","comments");
/// empty = all. Every result set is tenant-scoped by the global query filter and capped.</summary>
public record GlobalSearchQuery(string Query, IReadOnlyCollection<string> Types);

public interface IGlobalSearchHandler
{
    Task<Result<SearchResultsDto>> Handle(GlobalSearchQuery query, CancellationToken ct);
}

public class GlobalSearchHandler : IGlobalSearchHandler
{
    private const int PerTypeLimit = 10;

    private readonly IApplicationDbContext _db;

    public GlobalSearchHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result<SearchResultsDto>> Handle(GlobalSearchQuery query, CancellationToken ct)
    {
        var term = query.Query?.Trim() ?? string.Empty;
        if (term.Length < 2)
        {
            return Result<SearchResultsDto>.Fail("Search query must be at least 2 characters.");
        }

        bool Want(string type) => query.Types.Count == 0 || query.Types.Contains(type);

        var projects = Want("projects")
            ? await _db.Projects.AsNoTracking()
                .Where(p => p.Name.Contains(term) || p.Key.Contains(term))
                .OrderBy(p => p.Name).Take(PerTypeLimit)
                .Select(p => new SearchProjectItem(p.Id, p.Key, p.Name)).ToListAsync(ct)
            : new List<SearchProjectItem>();

        var epics = Want("epics")
            ? await _db.Epics.AsNoTracking()
                .Where(e => e.Name.Contains(term))
                .OrderBy(e => e.Name).Take(PerTypeLimit)
                .Select(e => new SearchEpicItem(e.Id, e.ProjectId, e.Name)).ToListAsync(ct)
            : new List<SearchEpicItem>();

        // Tasks: exact task-key match ranks first, then title/key partial matches.
        var tasks = Want("tasks")
            ? await _db.Tasks.AsNoTracking()
                .Where(t => t.TaskKey == term || t.Title.Contains(term) || t.TaskKey.Contains(term))
                .OrderByDescending(t => t.TaskKey == term)
                .ThenBy(t => t.Title)
                .Take(PerTypeLimit)
                .Select(t => new SearchTaskItem(t.Id, t.TaskKey, t.Title, t.ProjectId, t.Status.ToString(), t.ParentTaskId != null))
                .ToListAsync(ct)
            : new List<SearchTaskItem>();

        var users = Want("users")
            ? await _db.OrganizationUsers.AsNoTracking()
                .Where(ou => ou.User.FirstName.Contains(term) || ou.User.LastName.Contains(term) || ou.User.Email.Contains(term))
                .OrderBy(ou => ou.User.FirstName).Take(PerTypeLimit)
                .Select(ou => new SearchUserItem(ou.UserId, ou.User.FirstName + " " + ou.User.LastName, ou.User.Email))
                .ToListAsync(ct)
            : new List<SearchUserItem>();

        var comments = Want("comments")
            ? await _db.Comments.AsNoTracking()
                .Where(c => c.Body.Contains(term))
                .OrderByDescending(c => c.CreatedAt).Take(PerTypeLimit)
                .Select(c => new SearchCommentItem(c.Id, c.TaskId, c.Body.Length > 140 ? c.Body.Substring(0, 140) : c.Body))
                .ToListAsync(ct)
            : new List<SearchCommentItem>();

        return Result<SearchResultsDto>.Ok(new SearchResultsDto(term, projects, epics, tasks, users, comments));
    }
}
