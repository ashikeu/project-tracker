namespace ProjectTracker.Application.Features.SavedFilters.Dtos;

public record SavedFilterDto(
    Guid Id, Guid? ProjectId, string Name, string Scope, string FilterJson, DateTime CreatedAt);
