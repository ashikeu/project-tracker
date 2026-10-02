namespace ProjectTracker.Application.Features.Backlog.Queries.GetBacklog;

/// <summary>The product backlog for a project = top-level tasks not yet pulled into any sprint
/// (SprintId is null), ordered by their fractional Position. Paginated (see CLAUDE.md "No
/// unbounded GET endpoints").</summary>
public record GetBacklogQuery(Guid ProjectId, int Page, int PageSize);
