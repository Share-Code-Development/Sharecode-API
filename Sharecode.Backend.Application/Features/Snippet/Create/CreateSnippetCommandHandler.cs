using MediatR;
using Serilog;
using Sharecode.Backend.Application.Client;
using Sharecode.Backend.Application.Data;
using Sharecode.Backend.Application.Exceptions.Snippet;
using Sharecode.Backend.Domain.Entity.Profile;
using Sharecode.Backend.Domain.Entity.Snippet;
using Sharecode.Backend.Domain.Repositories;
using Sharecode.Backend.Utilities.MetaKeys;
using Sharecode.Backend.Utilities.RedisCache;

namespace Sharecode.Backend.Application.Features.Snippet.Create;

public class CreateSnippetCommandHandler(IHttpClientContext context, IUserRepository userRepository, IFileClient fileClient,ISnippetRepository snippetRepository, IUnitOfWork unitOfWork, ILogger logger) : IRequestHandler<CreateSnippetCommand, SnippetCreatedResponse>
{
    public async Task<SnippetCreatedResponse> Handle(CreateSnippetCommand request, CancellationToken cancellationToken)
    {
        var userIdentifier = await context.GetUserIdentifierAsync();
        User? user = null;
        if (userIdentifier.HasValue)
        {
            user = await userRepository.GetAsync(userIdentifier.Value, false, cancellationToken, false);
        }

        Domain.Entity.Snippet.Snippet snippet = new Domain.Entity.Snippet.Snippet()
        {
            Title = request.Title,
            Description = request.Description,
            Public = request.Public,
            Id = Guid.NewGuid(),
            Language = request.Language,
            PreviewCode = request.PreviewCode,
        };

        snippet.SetMeta(MetaKeys.SnippetKeys.LimitComments, false);
        
        snippet.CreateTags(request.Tags);
        if (user != null)
        {
            snippet.OwnerId = user.Id;
            //Create owner access if the snippet is not 
            if (!snippet.Public)
            {
                var ownerAccessControl = new SnippetAccessControl
                {
                    Snippet = snippet,
                    SnippetId = snippet.Id
                };
                ownerAccessControl.SetOwnership(snippet);
                snippet.AccessControls.Add(ownerAccessControl);
            }
        }
        
        snippet.Tags.Add(request.Language);
        var checksum = await fileClient.GetChecksum(request.Content);
        snippet.CheckSum = checksum;
        await snippetRepository.AddAsync(snippet, cancellationToken);
        await unitOfWork.CommitAsync(cancellationToken);
        var blob = await fileClient.UploadFileAsync(snippet.Id.ToString(), request.Content, false, cancellationToken);

        if (blob.Item1 == false || blob.Item2 == null)
            throw new FailedSnippetCreation("Failed to create the snippet. An unknown error occured!");

        //For My Snippets
        if(userIdentifier.HasValue)
            context.AddCacheKeyToInvalidate(CacheModules.UserSnippet, userIdentifier.Value.ToString());
        
        return new SnippetCreatedResponse()
        {
            SnippetId = snippet.Id
        };
    }
}