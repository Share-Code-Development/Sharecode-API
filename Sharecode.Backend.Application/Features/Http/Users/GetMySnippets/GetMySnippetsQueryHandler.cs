using MediatR;
using Serilog;
using Sharecode.Backend.Application.Client;
using Sharecode.Backend.Application.Service;
using Sharecode.Backend.Domain.Repositories;

namespace Sharecode.Backend.Application.Features.Http.Users.GetMySnippets;

public class GetMySnippetsQueryHandler(IUserRepository userRepository, IUserService userService ,IHttpClientContext context, ILogger logger) : IRequestHandler<GetMySnippetsQuery, GetMySnippetsResponse>
{
    public async Task<GetMySnippetsResponse> Handle(GetMySnippetsQuery request, CancellationToken cancellationToken)
    {
        var userIdentifier = await context.GetUserIdentifierAsync();
        var listUserSnippets = await userService.ListUserSnippetsAsync(userIdentifier!.Value, request.OnlyOwned, request.RecentSnippets, request.Skip, request.Take, request.Order ?? "ASC", request.OrderBy ?? string.Empty, request.SearchQuery ?? string.Empty, cancellationToken);
        GetMySnippetsResponse response = new GetMySnippetsResponse(listUserSnippets)
        {
            Query = request
        };
        response.TotalCount = response.Entities.FirstOrDefault()?.TotalCount ?? 0;
        return response;
    }
}