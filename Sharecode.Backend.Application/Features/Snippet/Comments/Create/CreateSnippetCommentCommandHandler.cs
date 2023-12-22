using MediatR;

namespace Sharecode.Backend.Application.Features.Snippet.Comments.Create;

public class CreateSnippetCommentCommandHandler : IRequestHandler<CreateSnippetCommentCommand, CreateSnippetCommentResponse> 
{
    public Task<CreateSnippetCommentResponse> Handle(CreateSnippetCommentCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}