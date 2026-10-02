namespace ProjectTracker.Application.Features.Boards.Dtos;

public record BoardColumnDto(Guid Id, Guid ProjectId, string Name, string Color, int Order, int? WipLimit, string StatusMapping, bool IsCompletionColumn, int TaskCount);
