using ProjectTracker.Domain.Enums;

namespace ProjectTracker.Application.Features.Tasks.Commands.AddDependency;

public record AddDependencyCommand(Guid TaskId, Guid DependsOnTaskId, TaskDependencyType Type);
