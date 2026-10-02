using FluentValidation;

namespace ProjectTracker.Application.Features.Tasks.Commands.CreateTask;

public class CreateTaskValidator : AbstractValidator<CreateTaskCommand>
{
    public CreateTaskValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(500);
        RuleFor(x => x.Description).MaximumLength(20000);
        RuleFor(x => x.StoryPoints).GreaterThanOrEqualTo(0).When(x => x.StoryPoints is not null);
        RuleFor(x => x.EstimatedHours).GreaterThanOrEqualTo(0).When(x => x.EstimatedHours is not null);
        RuleFor(x => x.DueDate).GreaterThanOrEqualTo(x => x.StartDate!.Value)
            .When(x => x.StartDate is not null && x.DueDate is not null)
            .WithMessage("Due date must be on or after the start date.");
    }
}
