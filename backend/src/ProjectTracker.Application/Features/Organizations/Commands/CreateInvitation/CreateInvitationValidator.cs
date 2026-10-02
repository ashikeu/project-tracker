using FluentValidation;

namespace ProjectTracker.Application.Features.Organizations.Commands.CreateInvitation;

public class CreateInvitationValidator : AbstractValidator<CreateInvitationCommand>
{
    public CreateInvitationValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.RoleId).NotEmpty();
    }
}
