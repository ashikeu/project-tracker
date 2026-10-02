namespace ProjectTracker.Application.Features.Epics.Dtos;

public record EpicDto(
    Guid Id, Guid ProjectId, string Name, string? Description, Guid? OwnerUserId, string? OwnerName,
    string Status, string Priority, string? Color, string? Icon,
    DateOnly? StartDate, DateOnly? EndDate, int TaskCount, int DoneTaskCount, DateTime CreatedAt);
