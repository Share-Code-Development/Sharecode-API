namespace Sharecode.Backend.Api.Middleware;

public static class CorsMiddlewareExtensions
{
    public static IApplicationBuilder UseCustomCors(this IApplicationBuilder app)
    {
        return app.Use((context, next) =>
        {
            var corsPolicyName = GetCorsPolicyBasedOnOrigin(context.Request);

            // Apply the determined CORS policy
            context.Response.Headers.Add("Access-Control-Allow-Origin", corsPolicyName);

            // If it's a preflight request, we need to return here
            if (context.Request.Method == "OPTIONS")
            {
                context.Response.Headers.Add("Access-Control-Allow-Methods", "GET, POST, PUT, DELETE, OPTIONS");
                context.Response.Headers.Add("Access-Control-Allow-Headers", "Content-Type, Authorization");
                context.Response.StatusCode = 200;
                return Task.CompletedTask;
            }

            return next();
        });
    }

    private static string GetCorsPolicyBasedOnOrigin(HttpRequest request)
    {
        // Logic to determine which policy to use
        var origin = request.Headers["Origin"].ToString();
        if (!string.IsNullOrEmpty(origin))
        {
            if (origin == "http://localhost:4000")
                return "LocalLink";
            else if (origin == "https://sharecodeapp.onrender.com")
                return "DeployedLink";
        }

        return "default"; // Default policy or an empty string
    }
}