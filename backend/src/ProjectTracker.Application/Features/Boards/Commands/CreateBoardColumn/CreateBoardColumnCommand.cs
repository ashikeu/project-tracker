using ProjectTracker.Domain.Enums;

namespace ProjectTracker.Application.Features.Boards.Commands.CreateBoardColumn;

public record CreateBoardColumnCommand(Guid ProjectId, string Name, string Color, int? WipLimit, TaskItemStatus StatusMapping, bool IsCompletionColumn);
