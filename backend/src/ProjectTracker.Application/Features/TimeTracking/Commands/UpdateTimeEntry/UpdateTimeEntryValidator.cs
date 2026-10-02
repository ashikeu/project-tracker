using FluentValidation;

namespace ProjectTracker.Application.Features.TimeTracking.Commands.UpdateTimeEntry;

public class UpdateTimeEntryValidator : AbstractValidator<UpdateTimeEntryCommand>
{
    public UpdateTimeEntryValidator()
    {
        RuleFor(x => x.DurationMinutes).GreaterThan(0).LessThanOrEqualTo(1440);
        RuleFor(x => x.Description).MaximumLength(1000);
    }
}
