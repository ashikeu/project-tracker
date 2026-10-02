using FluentValidation;

namespace ProjectTracker.Application.Features.Organizations.Commands.UpdateOrganization;

public class UpdateOrganizationValidator : AbstractValidator<UpdateOrganizationCommand>
{
    public UpdateOrganizationValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
    }
}
