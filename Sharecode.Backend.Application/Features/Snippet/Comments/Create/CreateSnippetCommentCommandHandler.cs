using MediatR;
using Sharecode.Backend.Application.Client;
using Sharecode.Backend.Application.Data;
using Sharecode.Backend.Application.Exceptions.Snippet;
using Sharecode.Backend.Application.Service;
using Sharecode.Backend.Domain.Entity.Snippet;
using Sharecode.Backend.Domain.Enums;
using Sharecode.Backend.Domain.Repositories;
using Sharecode.Backend.Utilities.MetaKeys;

namespace Sharecode.Backend.Application.Features.Snippet.Comments.Create;

public class CreateSnippetCommentCommandHandler(IHttpClientContext context, IUnitOfWork unitOfWork ,ISnippetRepository snippetRepository ,ISnippetService service ,ISnippetService snippetService, ISnippetCommentRepository snippetCommentRepository, ISnippetLineCommentRepository snippetLineCommentRepository) : IRequestHandler<CreateSnippetCommentCommand, CreateSnippetCommentResponse> 
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
            throw new FailedCommentCreationException("Unknown snippet has been send to create comment on!");

        var limitComments = snippet.ReadMeta<bool?>(MetaKeys.SnippetKeys.LimitComments);
        if (!limitComments.HasValue)
        {
            snippet.SetMeta(MetaKeys.SnippetKeys.LimitComments, false);
            limitComments = false;
        }
        
        if (limitComments.Value)
        {
            throw new FailedCommentCreationException("Commenting on this snippet is currently disabled.");
        }

        var snippetComment = new SnippetComment()
        {
            SnippetId = request.SnippetId,
            Text = request.Text,
            UserId = userId,
            ParentCommentId = null,
            Reactions = [],
            Id = Guid.NewGuid()
        };

        await snippetCommentRepository.AddAsync(snippetComment, token: cancellationToken);
        await unitOfWork.CommitAsync(cancellationToken);
        context.AddCacheKeyToInvalidate("snippet-comment", request.SnippetId.ToString());
        return new CreateSnippetCommentResponse()
        {
            Id = snippetComment.Id
        };
    }

    private async Task<CreateSnippetCommentResponse> CreateSnippetReplyCommentAsync(CreateSnippetCommentCommand request,
        Guid userId, CancellationToken cancellationToken)
    {
        return null;
    }
}