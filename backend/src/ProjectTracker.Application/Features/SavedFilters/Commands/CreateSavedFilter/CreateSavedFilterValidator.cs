using FluentValidation;

namespace ProjectTracker.Application.Features.SavedFilters.Commands.CreateSavedFilter;

public class CreateSavedFilterValidator : AbstractValidator<CreateSavedFilterCommand>
{
    public CreateSavedFilterValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(150);
        RuleFor(x => x.Scope).MaximumLength(40);
    }
}
