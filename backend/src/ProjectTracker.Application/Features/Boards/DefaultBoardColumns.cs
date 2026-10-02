using ProjectTracker.Domain.Entities;
using ProjectTracker.Domain.Enums;

namespace ProjectTracker.Application.Features.Boards;

/// <summary>The default column set every new project starts with (planning.md §12) — organizations
/// can rename/reorder/recolor/remove/add from here since BoardColumn is fully editable per project.</summary>
public static class DefaultBoardColumns
{
    public static List<BoardColumn> Build(Guid projectId)
    {
        (string Name, string Color, TaskItemStatus Status, bool IsCompletion)[] defaults =
        {
            ("Backlog", "#94a3b8", TaskItemStatus.Backlog, false),
            ("To Do", "#60a5fa", TaskItemStatus.Todo, false),
            ("In Progress", "#fbbf24", TaskItemStatus.InProgress, false),
            ("In Review", "#a78bfa", TaskItemStatus.InReview, false),
            ("Testing", "#f472b6", TaskItemStatus.Testing, false),
            ("Done", "#34d399", TaskItemStatus.Done, true)
        };

        var columns = new List<BoardColumn>();
        for (var i = 0; i < defaults.Length; i++)
        {
            var (name, color, status, isCompletion) = defaults[i];
            columns.Add(new BoardColumn
            {
                ProjectId = projectId,
                Name = name,
                Color = color,
                Order = i,
                StatusMapping = status,
                IsCompletionColumn = isCompletion
            });
        }
        return columns;
    }
}
