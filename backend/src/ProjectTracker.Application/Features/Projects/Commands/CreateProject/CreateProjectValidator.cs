using FluentValidation;

namespace ProjectTracker.Application.Features.Projects.Commands.CreateProject;

public class CreateProjectValidator : AbstractValidator<CreateProjectCommand>
{
    public CreateProjectValidator()
    {
        RuleFor(x => x.Key).NotEmpty().Length(2, 10)
            .Matches("^[A-Z][A-Z0-9]*$").WithMessage("Key must be uppercase letters/digits and start with a letter, e.g. 'ERP'.");
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Description).MaximumLength(4000);
        RuleFor(x => x.TargetDate).GreaterThanOrEqualTo(x => x.StartDate!.Value)
            .When(x => x.StartDate is not null && x.TargetDate is not null)
            .WithMessage("Target date must be on or after the start date.");
    }
}
