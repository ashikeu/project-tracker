using FluentValidation;

namespace ProjectTracker.Application.Features.Comments.Commands.CreateComment;

public class CreateCommentValidator : AbstractValidator<CreateCommentCommand>
{
    public CreateCommentValidator()
    {
        RuleFor(x => x.Body).NotEmpty().MaximumLength(10000);
    }
}
