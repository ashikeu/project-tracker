using FluentValidation;

namespace ProjectTracker.Application.Features.Teams.Commands.CreateTeam;

public class CreateTeamValidator : AbstractValidator<CreateTeamCommand>
{
    public CreateTeamValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(150);
        RuleFor(x => x.Description).MaximumLength(1000);
    }
}
