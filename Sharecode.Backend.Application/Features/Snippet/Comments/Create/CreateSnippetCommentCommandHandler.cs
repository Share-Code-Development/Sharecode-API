using MediatR;
using Sharecode.Backend.Application.Client;
using Sharecode.Backend.Application.Exceptions.Snippet;
using Sharecode.Backend.Application.Service;
using Sharecode.Backend.Domain.Entity.Snippet;
using Sharecode.Backend.Domain.Enums;
using Sharecode.Backend.Domain.Exceptions;
using Sharecode.Backend.Domain.Repositories;

namespace Sharecode.Backend.Application.Features.Snippet.Comments.Create;

public class CreateSnippetCommentCommandHandler(IHttpClientContext context, ISnippetRepository snippetRepository ,ISnippetService service ,ISnippetService snippetService, ISnippetCommentRepository snippetCommentRepository, ISnippetLineCommentRepository snippetLineCommentRepository) : IRequestHandler<CreateSnippetCommentCommand, CreateSnippetCommentResponse> 
{
    public async Task<CreateSnippetCommentResponse> Handle(CreateSnippetCommentCommand request, CancellationToken cancellationToken)
    {
        var userIdentifier = await context.GetUserIdentifierAsync();
        if (!userIdentifier.HasValue)
            throw new FailedCommentCreationException("Failed to create comment since the user is unknown!");

        return (request.CommentType) switch
        {
            SnippetCommentType.SnippetComment => await CreateSnippetCommentAsync(request, userIdentifier.Value,
                cancellationToken),
            SnippetCommentType.ReplyComment => await CreateSnippetReplyCommentAsync(request, userIdentifier.Value,
                cancellationToken),
            _ => throw new FailedCommentCreationException("Unknown comment type has been passed")
        };
    }

    private async Task<CreateSnippetCommentResponse> CreateSnippetCommentAsync(CreateSnippetCommentCommand request, Guid userId, CancellationToken cancellationToken)
    {
        var snippet = await snippetRepository.GetAsync(request.SnippetId, includeSoftDeleted: false, track: true, token: cancellationToken);
        if (snippet == null)
            throw new FailedCommentCreationException("Unknown snippet has been said to create comment on!");
        
        var snippetComment = new SnippetComment()
        {
            SnippetId = request.SnippetId,
            Text = request.Text,
            UserId = userId,
            ParentCommentId = null
        };

        return null;
    }

    private async Task<CreateSnippetCommentResponse> CreateSnippetReplyCommentAsync(CreateSnippetCommentCommand request,
        Guid userId, CancellationToken cancellationToken)
    {
        return null;
    }
}