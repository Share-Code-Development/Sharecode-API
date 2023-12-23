using System.Web;
using Sharecode.Backend.Utilities.Configuration;

namespace Sharecode.Backend.Utilities.Extensions;

public static class FrontendConfigurationExtension
{
    public static string CreateUrlFromBase(this Sharecode.Backend.Utilities.Configuration.FrontendConfiguration configuration, Dictionary<string, string?>? query, params string[] route)
    {
        // Create the base Uri
        var uriBuilder = new UriBuilder(configuration.Base);

        // Append route to base Uri
        var path = string.Join("/", route.Select(HttpUtility.UrlEncode));
        uriBuilder.Path = path;

        // If we have a query, append it to Uri
        if (query != null)
        {
            var queryString = HttpUtility.ParseQueryString(string.Empty);
            foreach (var item in query)
            {
                queryString[item.Key] = item.Value;
            }
            uriBuilder.Query = queryString.ToString();
        }

        return uriBuilder.ToString();
    }
}
