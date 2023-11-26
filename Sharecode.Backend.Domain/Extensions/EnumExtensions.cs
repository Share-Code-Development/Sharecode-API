using System.Web;
using Sharecode.Backend.Domain.Enums;
using Sharecode.Backend.Utilities.Extensions;

namespace Sharecode.Backend.Domain.Extensions;

public static class EnumExtensions
{
    public static string CreateGatewayUrl(this GatewayRequestType requestType, string baseUrl, Guid identifier,
        Dictionary<string, string>? queryParameters = null)
    {
        UriBuilder uriBuilder = new UriBuilder(baseUrl)
        {
            Path = $"gateway/{requestType.ToString().ToHyphenatedLower()}/{identifier.ToString()}"
        };

        if (queryParameters != null && queryParameters.Any())
        {
            var queryString = HttpUtility.ParseQueryString(string.Empty);
            foreach (var param in queryParameters)
            {
                queryString[param.Key] = param.Value;
            }
            uriBuilder.Query = queryString.ToString();
        }

        return uriBuilder.Uri.AbsoluteUri;
    }
}