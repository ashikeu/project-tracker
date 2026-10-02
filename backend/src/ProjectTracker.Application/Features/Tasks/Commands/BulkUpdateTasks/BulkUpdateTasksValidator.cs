using FluentValidation;

namespace ProjectTracker.Application.Features.Tasks.Commands.BulkUpdateTasks;

public class BulkUpdateTasksValidator : AbstractValidator<BulkUpdateTasksCommand>
{
    public BulkUpdateTasksValidator()
    {
        RuleFor(x => x.TaskIds).NotEmpty().Must(ids => ids.Count <= 200)
            .WithMessage("Bulk operations are limited to 200 tasks at a time.");
    }
}
