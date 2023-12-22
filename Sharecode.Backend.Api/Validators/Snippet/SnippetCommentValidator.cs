using FluentValidation;
using Sharecode.Backend.Application.Features.Snippet.Comments.Create;
using Sharecode.Backend.Domain.Enums;

namespace Sharecode.Backend.Api.Validators.Snippet;

public class CreateSnippetCommentValidator : AbstractValidator<CreateSnippetCommentCommand>
{
    public CreateSnippetCommentValidator()
    {
        RuleFor(x => x.Text)
            .NotEmpty()
            .MaximumLength(1000)
            .WithMessage("Comments should not be empty and should be less than 1000");

        RuleFor(x => x.CommentType)
            .NotNull()
            .WithMessage("CommentType should be specified");

        When(x => x.CommentType == SnippetCommentType.SnippetComment, () =>
        {
            RuleFor(x => x.ParentCommentId)
                .Null()
                .WithMessage($"Snippet comments shouldn't have a reply message");
        });

        When(x => x.CommentType == SnippetCommentType.ReplyComment, () =>
        {
            RuleFor(x => x.ParentCommentId)
                .NotNull()
                .WithMessage($"ParentCommentId should be specified for reply comments");
        });

        When(x => x.CommentType == SnippetCommentType.LineComment, () =>
        {
            RuleFor(x => x.LineNumber)
                .NotNull()
                .WithMessage($"No line no is specified");
            
            RuleFor(x => x.ParentCommentId)
                .Null()
                .WithMessage($"Line comments shouldn't have a reply message");
        });
    }
}