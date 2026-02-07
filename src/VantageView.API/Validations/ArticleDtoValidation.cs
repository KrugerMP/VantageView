using FluentValidation;
using VantageView.API.Models;

namespace VantageView.API.Validations;

public class CreateArticleDtoValidator : AbstractValidator<CreateArticleDto>
{
    public CreateArticleDtoValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("A title must be provided")
            .MinimumLength(2).WithMessage("A title must be longer than 2 characters")
            .MaximumLength(215).WithMessage("A title must be at most 215 characters");

        RuleFor(x => x.Summary)
            .NotEmpty().WithMessage("A summary must be provided");

        RuleFor(x => x.Content)
            .NotEmpty().WithMessage("Content must be provided");

        RuleFor(x => x.Author)
            .NotEmpty().WithMessage("An author must be provided");
    }
}

public class UpdateArticleDtoValidator : AbstractValidator<UpdateArticleDto>
{
    public UpdateArticleDtoValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("A title must be provided")
            .MinimumLength(2).WithMessage("A title must be longer than 2 characters")
            .MaximumLength(215).WithMessage("A title must be at most 215 characters");

        RuleFor(x => x.Summary)
            .NotEmpty().WithMessage("A summary must be provided");

        RuleFor(x => x.Content)
            .NotEmpty().WithMessage("Content must be provided");
    }
}