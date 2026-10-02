using ProjectTracker.Domain.Enums;

namespace ProjectTracker.Application.Features.Epics.Commands.CreateEpic;

public record CreateEpicCommand(
    Guid ProjectId, string Name, string? Description, Guid? OwnerUserId, Priority Priority,
    string? Color, string? Icon, DateOnly? StartDate, DateOnly? EndDate);
