using MediatR;
using Sharecode.Backend.Application.Client;
using Sharecode.Backend.Application.Data;
using Sharecode.Backend.Application.Exceptions.Snippet;
using Sharecode.Backend.Application.Service;
using Sharecode.Backend.Domain.Entity.Snippet;
using Sharecode.Backend.Domain.Enums;
using Sharecode.Backend.Domain.Repositories;
using Sharecode.Backend.Utilities.Extensions;
using Sharecode.Backend.Utilities.MetaKeys;
using Sharecode.Backend.Utilities.RedisCache;

namespace Sharecode.Backend.Application.Features.Http.Snippet.Comments.Create;

public class CreateSnippetCommentCommandHandler(IHttpClientContext context, IUnitOfWork unitOfWork ,ISnippetRepository snippetRepository ,ISnippetService service ,ISnippetService snippetService, ISnippetCommentRepository snippetCommentRepository, ISnippetLineCommentRepository snippetLineCommentRepository) : IRequestHandler<CreateSnippetCommentCommand, CreateSnippetCommentResponse> 
{
    public async Task<CreateSnippetCommentResponse> Handle(CreateSnippetCommentCommand request, CancellationToken cancellationToken)
    {
        var userIdentifier = await context.GetUserIdentifierAsync();
        if (!userIdentifier.HasValue)
            throw new FailedCommentCreationException("Failed to create comment since the user is unknown!");
        
        var snippet = await snippetRepository.GetAsync(request.SnippetId, includeSoftDeleted: false, track: true, token: cancellationToken);
        if (snippet == null)
            throw new FailedCommentCreationException("Unknown snippet has been send to create comment on!");

        var limitComments = snippet.ReadMeta<bool?>(MetaKeys.SnippetKeys.LimitComments);
        if (!limitComments.HasValue)
        {
            snippet.SetMeta(MetaKeys.SnippetKeys.LimitComments, false);
            limitComments = false;
        }

        var restrictedUsers = snippet.ReadMeta<HashSet<Guid>?>(MetaKeys.SnippetKeys.CommentRestriction);
        if (restrictedUsers != null)
        {
            if (restrictedUsers.Contains(userIdentifier.Value))
                throw new BlockedCommentCreationException();
        }
        
        if (limitComments.Value)
        {
            throw new FailedCommentCreationException("Commenting on this snippet is currently disabled.");
        }

        return (request.CommentType) switch
        {
            SnippetCommentType.SnippetComment => await CreateSnippetCommentAsync(request, userIdentifier.Value, snippet,
                cancellationToken),
            SnippetCommentType.ReplyComment => await CreateSnippetReplyCommentAsync(request, userIdentifier.Value, snippet,
                cancellationToken),
            SnippetCommentType.LineComment => await CreateSnippetLineCommentAsync(request, userIdentifier.Value, snippet,
                cancellationToken),
            _ => throw new FailedCommentCreationException("Unknown comment type has been passed")
        };
    }

    private async Task<CreateSnippetCommentResponse> CreateSnippetCommentAsync(CreateSnippetCommentCommand request, Guid userId, Domain.Entity.Snippet.Snippet snippet, CancellationToken cancellationToken)
    {
        var snippetComment = new SnippetComment()
        {
            SnippetId = request.SnippetId,
            Text = request.Text,
            UserId = userId,
            ParentCommentId = null,
            Reactions = [],
            Id = Guid.NewGuid(),
            Mentions = request.Text.ExtractMentionableUsers()
        };

        await snippetCommentRepository.AddAsync(snippetComment, token: cancellationToken);
        await unitOfWork.CommitAsync(cancellationToken);
        context.AddCacheKeyToInvalidate("snippet-comment", request.SnippetId.ToString());
        return new CreateSnippetCommentResponse()
        {
            Id = snippetComment.Id,
            SnippetId = snippet.Id,
            ParentCommentId = null,
            LineNumber = null
        };
    }

    private async Task<CreateSnippetCommentResponse> CreateSnippetReplyCommentAsync(CreateSnippetCommentCommand request,
        Guid userId, Domain.Entity.Snippet.Snippet snippet,  CancellationToken cancellationToken)
    {
        var replySnippetComment = new SnippetComment()
        {
            SnippetId = request.SnippetId,
            Text = request.Text,
            UserId = userId,
            ParentCommentId = request.ParentCommentId!.Value,
            Reactions = [],
            Id = Guid.NewGuid(),
            Mentions = request.Text.ExtractMentionableUsers()
        };

        await snippetCommentRepository.AddAsync(replySnippetComment, cancellationToken);
        await unitOfWork.CommitAsync(cancellationToken);
        context.AddCacheKeyToInvalidate(CacheModules.SnippetComment, request.SnippetId.ToString(), request.ParentCommentId!.Value.ToString());
        return new CreateSnippetCommentResponse()
        {
            Id = replySnippetComment.Id,
            SnippetId = replySnippetComment.SnippetId,
            ParentCommentId = replySnippetComment.ParentCommentId,
            LineNumber = null
        };
    }
    
    private async Task<CreateSnippetCommentResponse> CreateSnippetLineCommentAsync(CreateSnippetCommentCommand request,
        Guid userId, Domain.Entity.Snippet.Snippet snippet, CancellationToken cancellationToken)
    {
        var lineComment = new SnippetLineComment()
        {
            SnippetId = request.SnippetId,
            Text = request.Text,
            UserId = userId,
            LineNumber = request.LineNumber!.Value,
            Id = Guid.NewGuid(),
            Mentions = request.Text.ExtractMentionableUsers()
        };

        await snippetLineCommentRepository.AddAsync(lineComment, cancellationToken);
        await unitOfWork.CommitAsync(cancellationToken);
        //Line comments are from snippets
        context.AddCacheKeyToInvalidate(CacheModules.Snippet, request.SnippetId.ToString());
        return new CreateSnippetCommentResponse()
        {
            Id = lineComment.Id,
            SnippetId = lineComment.SnippetId,
            ParentCommentId = null,
            LineNumber = request.LineNumber.Value
        };
    }
}