using FluentValidation;
using Sharecode.Backend.Application.Features.Http.Snippet.Create;


namespace Sharecode.Backend.Api.Validators.Snippet;

public class CreateSnippetCommandValidator : AbstractValidator<CreateSnippetCommand>
{
    public CreateSnippetCommandValidator()
    {
        RuleFor(x => x.Language)
            .NotNull()
            .WithMessage("Please provide a valid language");

        RuleFor(x => x.Language)
            .MaximumLength(20)
            .WithMessage("Language should not be greater than 20 characters");

        RuleFor(x => x.Description)
            .MaximumLength(500)
            .WithMessage("Description should not be larger than 500");

        RuleFor(x => x.Content)
            .NotNull()
            .NotEmpty()
            .WithMessage("The snippet should not be empty");

        RuleFor(x => x.Title)
            .NotNull()
            .WithMessage("Title should not be empty");

        RuleFor(x => x.Title)
            .MaximumLength(100)
            .WithMessage("Title should not be larger than 100 characters");

        When(x => x.Content.Length > 0, () =>
        {
            RuleFor(x => x.PreviewCode)
                .NotNull()
                .NotEmpty()
                .WithMessage("Please provide the preview code.");

            RuleFor(x => x.PreviewCode)
                .MaximumLength(500)
                .WithMessage("Please provide a preview code of characters less than 500");
        });
    }
}
