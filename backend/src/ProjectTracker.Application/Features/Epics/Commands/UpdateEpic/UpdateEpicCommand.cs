using ProjectTracker.Domain.Enums;

namespace ProjectTracker.Application.Features.Epics.Commands.UpdateEpic;

public record UpdateEpicCommand(
    Guid EpicId, string Name, string? Description, Guid? OwnerUserId, EpicStatus Status, Priority Priority,
    string? Color, string? Icon, DateOnly? StartDate, DateOnly? EndDate);
