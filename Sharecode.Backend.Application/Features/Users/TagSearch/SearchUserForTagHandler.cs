using MediatR;
using Sharecode.Backend.Application.Client;
using Sharecode.Backend.Application.Service;

namespace Sharecode.Backend.Application.Features.Users.TagSearch;

public class SearchUserForTagHandler(IUserService userService, IHttpClientContext clientContext) : IRequestHandler<SearchUsersForTagCommand, SearchUserForTagResponse>
{
    public async Task<SearchUserForTagResponse> Handle(SearchUsersForTagCommand request, CancellationToken cancellationToken)
    {
        bool includeDeleted = false;
        var usersToTagAsync = await userService.GetUsersToTagAsync(request.SearchQuery, request.Take, request.Skip, includeDeleted,
            request.ShouldEnableTagging, cancellationToken);
        
        return (SearchUserForTagResponse) new SearchUserForTagResponse(usersToTagAsync)
                .SetQuery(request)
            ;
    }
}