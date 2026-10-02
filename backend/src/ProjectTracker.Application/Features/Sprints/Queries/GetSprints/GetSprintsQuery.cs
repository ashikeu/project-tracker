using ProjectTracker.Domain.Enums;

namespace ProjectTracker.Application.Features.Sprints.Queries.GetSprints;

public record GetSprintsQuery(Guid ProjectId, SprintStatus? Status);
