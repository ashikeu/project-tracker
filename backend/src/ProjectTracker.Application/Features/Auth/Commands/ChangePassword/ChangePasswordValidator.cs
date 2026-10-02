using FluentValidation;

namespace ProjectTracker.Application.Features.Auth.Commands.ChangePassword;

public class ChangePasswordValidator : AbstractValidator<ChangePasswordCommand>
{
    public ChangePasswordValidator()
    {
        RuleFor(x => x.CurrentPassword).NotEmpty();
        RuleFor(x => x.NewPassword).NotEmpty().MinimumLength(8)
            .Matches("[A-Z]").Matches("[a-z]").Matches("[0-9]")
            .NotEqual(x => x.CurrentPassword).WithMessage("New password must differ from the current password.");
    }
}
