using FluentValidation;

namespace ProjectTracker.Application.Features.Epics.Commands.UpdateEpic;

public class UpdateEpicValidator : AbstractValidator<UpdateEpicCommand>
{
    public UpdateEpicValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Description).MaximumLength(4000);
        RuleFor(x => x.EndDate).GreaterThanOrEqualTo(x => x.StartDate!.Value)
            .When(x => x.StartDate is not null && x.EndDate is not null)
            .WithMessage("End date must be on or after the start date.");
    }
}
