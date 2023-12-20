using MediatR;
using Serilog;
using Sharecode.Backend.Application.Client;
using Sharecode.Backend.Application.Data;
using Sharecode.Backend.Domain.Entity.Profile;
using Sharecode.Backend.Domain.Repositories;

namespace Sharecode.Backend.Application.Features.Snippet.Create;

public class CreateSnippetCommandHandler(IHttpClientContext context, IUserRepository userRepository, ISnippetRepository snippetRepository, IUnitOfWork unitOfWork, ILogger logger) : IRequestHandler<CreateSnippetCommand, SnippetCreatedResponse>
{
    public async Task<SnippetCreatedResponse> Handle(CreateSnippetCommand request, CancellationToken cancellationToken)
    {
        var userIdentifier = await context.GetUserIdentifierAsync();
        User? user = null;
        if (userIdentifier.HasValue)
        {
            user = await userRepository.GetUserByIdIncludingAccountSettings(userIdentifier.Value, true, false,
                cancellationToken);
        }

        Domain.Entity.Snippet.Snippet snippet = new Domain.Entity.Snippet.Snippet()
        {
            Title = request.Title,
            Description = request.Description,
            Public = request.Public,
            Id = Guid.NewGuid(),
            Language = request.Language,
            PreviewCode = $"ABC"
        };

        if (user != null)
        {
            snippet.Owner = user;
            snippet.OwnerId = user.Id;
        }
        
        await snippetRepository.AddAsync(snippet, cancellationToken);
        await unitOfWork.CommitAsync(cancellationToken);

        return SnippetCreatedResponse.From(snippet);
    }
}