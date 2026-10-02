using FluentValidation;

namespace ProjectTracker.Application.Features.Epics.Commands.CreateEpic;

public class CreateEpicValidator : AbstractValidator<CreateEpicCommand>
{
    public CreateEpicValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Description).MaximumLength(4000);
        RuleFor(x => x.Color).MaximumLength(20);
        RuleFor(x => x.Icon).MaximumLength(50);
        RuleFor(x => x.EndDate).GreaterThanOrEqualTo(x => x.StartDate!.Value)
            .When(x => x.StartDate is not null && x.EndDate is not null)
            .WithMessage("End date must be on or after the start date.");
    }
}
