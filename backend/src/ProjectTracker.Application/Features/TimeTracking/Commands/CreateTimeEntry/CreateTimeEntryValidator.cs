using FluentValidation;

namespace ProjectTracker.Application.Features.TimeTracking.Commands.CreateTimeEntry;

public class CreateTimeEntryValidator : AbstractValidator<CreateTimeEntryCommand>
{
    public CreateTimeEntryValidator()
    {
        // 1440 minutes = 24h; a single manual entry longer than a day is almost certainly a typo.
        RuleFor(x => x.DurationMinutes).GreaterThan(0).LessThanOrEqualTo(1440);
        RuleFor(x => x.Description).MaximumLength(1000);
    }
}
