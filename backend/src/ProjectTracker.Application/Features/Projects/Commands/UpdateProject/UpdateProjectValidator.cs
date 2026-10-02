using FluentValidation;

namespace ProjectTracker.Application.Features.Projects.Commands.UpdateProject;

public class UpdateProjectValidator : AbstractValidator<UpdateProjectCommand>
{
    public UpdateProjectValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Description).MaximumLength(4000);
        RuleFor(x => x.TargetDate).GreaterThanOrEqualTo(x => x.StartDate!.Value)
            .When(x => x.StartDate is not null && x.TargetDate is not null)
            .WithMessage("Target date must be on or after the start date.");
    }
}
