namespace Sharecode.Backend.Application.Features.Http.Users.Metadata.List;

public class ListUserMetadataResponse
{
    public Dictionary<string, object> Responses { get; } = new();

    private ListUserMetadataResponse()
    {
    }

    public static ListUserMetadataResponse From(Dictionary<string, object> request, HashSet<string> requestedKeys)
    {
        ListUserMetadataResponse response = new();
        foreach (var requestedKey in requestedKeys)
        {
            if (request.TryGetValue(requestedKey, out var responseObj))
            {
                response.Responses.Add(requestedKey, responseObj);   
            }
        }

        return response;
    }
}