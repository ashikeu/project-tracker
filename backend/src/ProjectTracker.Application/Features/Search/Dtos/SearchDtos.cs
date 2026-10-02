namespace ProjectTracker.Application.Features.Search.Dtos;

public record SearchProjectItem(Guid Id, string Key, string Name);
public record SearchEpicItem(Guid Id, Guid ProjectId, string Name);
public record SearchTaskItem(Guid Id, string TaskKey, string Title, Guid ProjectId, string Status, bool IsSubtask);
public record SearchUserItem(Guid Id, string Name, string Email);
public record SearchCommentItem(Guid Id, Guid TaskId, string Snippet);

public record SearchResultsDto(
    string Query,
    IReadOnlyList<SearchProjectItem> Projects,
    IReadOnlyList<SearchEpicItem> Epics,
    IReadOnlyList<SearchTaskItem> Tasks,
    IReadOnlyList<SearchUserItem> Users,
    IReadOnlyList<SearchCommentItem> Comments);
