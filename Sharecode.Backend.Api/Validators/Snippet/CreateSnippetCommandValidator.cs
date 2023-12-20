using FluentValidation;
using Sharecode.Backend.Application.Features.Snippet;
using Sharecode.Backend.Application.Features.Snippet.Create;

namespace Sharecode.Backend.Api.Validators.Snippet;

public class CreateSnippetCommandValidator : AbstractValidator<CreateSnippetCommand>
{
    public CreateSnippetCommandValidator()
    {
        RuleFor(x => x.Language)
            .NotNull()
            .WithMessage("Please provide a valid language");

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
        
        
    }
}