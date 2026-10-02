using FluentValidation;

namespace ProjectTracker.Application.Features.Teams.Commands.UpdateTeam;

public class UpdateTeamValidator : AbstractValidator<UpdateTeamCommand>
{
    public UpdateTeamValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(150);
        RuleFor(x => x.Description).MaximumLength(1000);
    }
}
