using FluentValidation;
using VantageView.API.Models;

namespace VantageView.API.Validations;

/// <summary>
/// Validates <see cref="CreateArticleDto"/> when creating a new article.
/// </summary>
public class CreateArticleDtoValidator : AbstractValidator<CreateArticleDto>
{
    /// <summary>
    /// Initializes validation rules for create-article requests.
    /// </summary>
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

/// <summary>
/// Validates <see cref="UpdateArticleDto"/> when updating an existing article.
/// </summary>
public class UpdateArticleDtoValidator : AbstractValidator<UpdateArticleDto>
{
    /// <summary>
    /// Initializes validation rules for update-article requests.
    /// </summary>
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