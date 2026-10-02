using FluentValidation;

namespace ProjectTracker.Application.Features.Boards.Commands.CreateBoardColumn;

public class CreateBoardColumnValidator : AbstractValidator<CreateBoardColumnCommand>
{
    public CreateBoardColumnValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Color).NotEmpty().MaximumLength(20);
        RuleFor(x => x.WipLimit).GreaterThan(0).When(x => x.WipLimit is not null);
    }
}
