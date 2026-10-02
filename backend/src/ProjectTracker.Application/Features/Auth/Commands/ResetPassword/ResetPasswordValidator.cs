using FluentValidation;

namespace ProjectTracker.Application.Features.Auth.Commands.ResetPassword;

public class ResetPasswordValidator : AbstractValidator<ResetPasswordCommand>
{
    public ResetPasswordValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Token).NotEmpty();
        RuleFor(x => x.NewPassword).NotEmpty().MinimumLength(8)
            .Matches("[A-Z]").Matches("[a-z]").Matches("[0-9]");
    }
}
