using ProjectTracker.Domain.Enums;

namespace ProjectTracker.Application.Features.Boards.Commands.UpdateBoardColumn;

public record UpdateBoardColumnCommand(Guid ColumnId, string Name, string Color, int? WipLimit, TaskItemStatus StatusMapping, bool IsCompletionColumn);
