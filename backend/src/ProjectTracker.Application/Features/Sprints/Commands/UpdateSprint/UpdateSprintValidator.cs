using FluentValidation;

namespace ProjectTracker.Application.Features.Sprints.Commands.UpdateSprint;

public class UpdateSprintValidator : AbstractValidator<UpdateSprintCommand>
{
    public UpdateSprintValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Goal).MaximumLength(2000);
        RuleFor(x => x.EndDate).GreaterThanOrEqualTo(x => x.StartDate!.Value)
            .When(x => x.StartDate is not null && x.EndDate is not null)
            .WithMessage("End date must be on or after the start date.");
    }
}
