using MediatR;
using Sharecode.Backend.Application.Client;
using Sharecode.Backend.Application.Service;

namespace Sharecode.Backend.Application.Features.Live.Snippet.Joined;

public class JoinedSnippetLiveHandler(IHttpClientContext clientContext, IGroupStateManager groupStateManager, ISnippetService snippetService, IUserService userService) : IRequestHandler<JoinedSnippetEvent, JoinedSnippetResponse?>
{
    public async Task<JoinedSnippetResponse?> Handle(JoinedSnippetEvent request, CancellationToken cancellationToken)
    {
        var requestingUser = await clientContext.GetUserIdentifierAsync() ?? Guid.Empty;
        //If the requests doesn't have an authorization token.
        //Scenario - Public Snippets
        bool isRequestAnonymous = requestingUser == Guid.Empty;
        // Get the users access to work on snippets
        //If there is no authorization token, the request would be sent with Empty Guid, which won't match the select
        //statement with in the access control table.
        var snippetAccess = await snippetService.GetSnippetAccess(request.SnippetId, requestingUser , false);
        //If the user doesn't have any access to that particular snippet, then return null. If the snippet is public
        //the fn will return read access
        if (!snippetAccess.Any())
        {
            return null;
        }

        //Get all the existing members in the signalR group
        var currentMembers = await groupStateManager.Members(request.SnippetId.ToString(), cancellationToken);
        //The way we store it in SignalR is, if there is a validated user, he/she will be a Guid, if its a string, it will
        //be an anonymous user.
        var loggedInUsers = currentMembers.Values.Select(x =>
            {
                bool success = Guid.TryParse(x, out var parsed);
                return new { success, parsed };
            })
            .Where(x => x.success)
            .Select(x => x.parsed)
            .ToHashSet();
        //If the requesting user is logged in, then also add him/her too, so that we can get the logo and all of this newly
        //joined user too
        if (!isRequestAnonymous)
        {
            loggedInUsers.Add(requestingUser);
        }

        //Get User Profile Information
        var userEnumerable = await userService.GetUsersProfileInformationAsync(loggedInUsers, cancellationToken);
        var activeUsers = userEnumerable
            .Select(x =>
                new ActiveSnippetUsersDto()
                {
                    Id = x.Id,
                    FullName = x.FullName,
                    ProfilePicture = x.ProfilePicture
                })
            .ToDictionary(x => x.Id!.Value, x => x);

        List<ActiveSnippetUsersDto> responseUsers = [];
        //Loop through the current members in the group
        foreach (var (connectionId, userIdentifier) in currentMembers)
        {
            //If the current member in the group is a Guid, that means he/she was a logged in user
            if (Guid.TryParse(userIdentifier, out var uniqueUserId))
            {
                //Check whether such a user was there in the response returned from Db
                if (activeUsers.TryGetValue(uniqueUserId, out var activeUser))
                {
                    //If there is attach connectionId and also add to the return array
                    activeUser.ConnectionId = connectionId;
                    responseUsers.Add(activeUser);
                }
            }
            else
            {
                //In this else case, it means the user is a non-logged in user
                //In this case, only attach the name and connection Id we have
                var anonymousUser = new ActiveSnippetUsersDto()
                {
                    ConnectionId = connectionId,
                    FullName = userIdentifier,
                    Id = null,
                    ProfilePicture = null
                };
                responseUsers.Add(anonymousUser);
            }
        }
        
        var response = new JoinedSnippetResponse()
        {
            JoinedUserAccesses = snippetAccess.ToControlModel(),
            SnippetId = request.SnippetId,
            JoinedUserId = Guid.Empty,
            ActiveUsers = responseUsers
        };

        //If the requesting user was not logged in or if there is no data
        //associated with the requested user in the dbResponse
        //Set him as an anonymous User, also add him in the array of current users
        if (isRequestAnonymous || !activeUsers.ContainsKey(requestingUser))
        {
            response.JoinedUserName = "Anonymous User";
            var anonymousUser = new ActiveSnippetUsersDto()
            {
                ConnectionId = request.ConnectionId,
                FullName = response.JoinedUserName,
                Id = null,
                ProfilePicture = null
            };
            responseUsers.Add(anonymousUser);
        }
        else
        {
            //If there is data, Set the UserName and UserId for the newly joined user
            //Also add him in the current users array
            var activeSnippetUsersDto = activeUsers[requestingUser];
            activeSnippetUsersDto.ConnectionId = request.ConnectionId;
            responseUsers.Add(activeSnippetUsersDto);
            response.JoinedUserName = activeSnippetUsersDto.FullName;
            response.JoinedUserId = activeSnippetUsersDto.Id;
        }
        
        return response;
    }
}