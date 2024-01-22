using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.RateLimiting;
using FluentValidation;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Serialization;
using Sharecode.Backend.Api.Filters;
using Sharecode.Backend.Api.Service;
using Sharecode.Backend.Api.SignalR;
using Sharecode.Backend.Application;
using Sharecode.Backend.Application.Client;
using Sharecode.Backend.Infrastructure;
using Sharecode.Backend.Infrastructure.Client;
using Sharecode.Backend.Presentation;
using Sharecode.Backend.Utilities;
using Sharecode.Backend.Utilities.Configuration;
using Sharecode.Backend.Utilities.KeyValue;

namespace Sharecode.Backend.Api.Extensions;

public static class BootstrapExtensions
{

    public static IServerSession CreateAndRegisterSession(this WebApplicationBuilder builder)
    {
        var value = builder.Configuration.GetValue<string?>("ServerId") ?? string.Empty;

        IServerSession serverSession = new ServerSession(value);
        builder.Services.AddSingleton(serverSession);
        return serverSession;
    }
    
    public static IServiceCollection AddShareCodeRateLimiting(this IServiceCollection service)
    {
        service.AddRateLimiter(rateLimiterOptions =>
        {
            rateLimiterOptions.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            rateLimiterOptions.AddFixedWindowLimiter("fixed", options =>
            {
                options.PermitLimit = 10;
                options.Window = TimeSpan.FromSeconds(10);
                options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                options.QueueLimit = 5;
            });
    
            rateLimiterOptions.AddSlidingWindowLimiter("sliding", options =>
            {
                options.PermitLimit = 10;
                options.Window = TimeSpan.FromSeconds(10);
                options.SegmentsPerWindow = 2;
                options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                options.QueueLimit = 5;
            });
    
            rateLimiterOptions.AddTokenBucketLimiter("token", options =>
            {
                options.TokenLimit = 100;
                options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                options.QueueLimit = 5;
                options.ReplenishmentPeriod = TimeSpan.FromSeconds(10);
                options.TokensPerPeriod = 20;
                options.AutoReplenishment = true;
            });
    
            rateLimiterOptions.AddConcurrencyLimiter("concurrency", options =>
            {
                options.PermitLimit = 10;
                options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                options.QueueLimit = 5;
                
            });
        });
        return service;
    }
    
    public static IServiceCollection BindConfigurationEntries(this IServiceCollection service, ConfigurationManager configurationManager)
    {
        service.Configure<KeyValueConfiguration>(options => 
            configurationManager.GetSection("CloudFlareKV").Bind(options));
        service.Configure<JwtConfiguration>(options => 
            configurationManager.GetSection("JWT").Bind(options));
        service.Configure<LocalDirectoryConfiguration>(options =>
            configurationManager.GetSection("LocalDirectory").Bind(options));
        service.Configure<GatewayLimitConfiguration>(options =>
            configurationManager.GetSection("GatewayLimit").Bind(options)
        );
        service.Configure<FrontendConfiguration>(options =>
        {
            var section = configurationManager.GetSection("Frontend");
            section.Bind(options);
        });
        service.Configure<FileClientConfiguration>(options =>
        {
            configurationManager.GetSection("FileClient").Bind(options);
        });
        return service;
    }

    public static IServiceCollection RegisterCoreServices(this IServiceCollection service, IWebHostEnvironment environment, IConfiguration configuration)
    {
        service.AddSingleton<IKeyValueClient, KeyValueClient>();
        service.AddHttpContextAccessor();
        service.AddScoped<IHttpClientContext, HttpClientContext>();
        service.AddValidatorsFromAssembly(typeof(BootstrapExtensions).Assembly);
        service.AddControllers(options => {
                options.Filters.Add(new ApiRequestFilter());
            }).AddNewtonsoftJson(jsonOptions =>
            {
                jsonOptions.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            });

        if (environment.IsDevelopment())
        {
            IdentityModelEventSource.ShowPII = true;
            Console.WriteLine($"Enabled to show details for authentication schema with in development env");
        }
        
        service.AddSignalR(x =>
        {
            x.EnableDetailedErrors = true;
            x.KeepAliveInterval = TimeSpan.FromSeconds(20); 
            x.HandshakeTimeout = TimeSpan.FromSeconds(20);
        });
        return service;
    }

    public static IServiceCollection RegisterLayeredServices(this IServiceCollection serviceCollection, Namespace keyValueNamespace, WebApplicationBuilder builder)
    {
        serviceCollection.RegisterInfrastructureLayer(keyValueNamespace, builder.Configuration, builder.Environment.IsDevelopment())
            .RegisterApplicationLayer()
            .RegisterPresentationLayer();

        return serviceCollection;
    }

    public static IServiceCollection RegisterCors(this IServiceCollection service)
    {
        Regex urlRegex = new Regex("^https:\\/\\/sharecodeapp(-pr-\\d+)?\\.onrender\\.com$", RegexOptions.Compiled);
        service.AddCors(options =>
        {
            options.AddPolicy("DeployedLink", builder =>
            {
                builder.AllowCredentials()
                    .AllowAnyHeader()
                    .SetIsOriginAllowed((url) =>
                    {
                        if (url is "https://sharecodeapp.onrender.com" or "http://localhost:4000")
                            return true;
                        
                        return urlRegex.Match(url).Success;
                    })
                    .AllowAnyMethod();
                
            });
        });
        return service;
    }

    public static IKeyValueClient GetKeyValueClient(this IServiceCollection serviceCollection)
    {
        ServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();
        IKeyValueClient keyValueClient = serviceProvider.GetRequiredService<IKeyValueClient>();
        return keyValueClient;
    }
    
    public static IServiceCollection BuildAuthenticationSchema(this IServiceCollection serviceCollection, IConfiguration configuration, Namespace keyValueNamespace)
    {
        serviceCollection.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(bearerOptions =>
            {
                var value = keyValueNamespace.Of(KeyVaultConstants.JwtSecretKey)?.Value;
                var encryptionKey = keyValueNamespace.Of(KeyVaultConstants.JwtAccessTokenEncryptionKey)?.Value;
                if (value != null)
                    bearerOptions.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateIssuerSigningKey = true,
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidIssuer = configuration["JWT:Issuer"],
                        ValidAudience = configuration["JWT:Audience"],
                        TokenDecryptionKey = new SymmetricSecurityKey(Convert.FromBase64String(encryptionKey)),
                        IssuerSigningKey =
                            new SymmetricSecurityKey(
                                Encoding.ASCII.GetBytes(value))
                    };

                //SignalR Configuration
                bearerOptions.Events = new JwtBearerEvents()
                {
                    OnMessageReceived = context =>
                    {
                        //Get the access token
                        var signalRToken = context.Request.Query["access_token"];
                        var requestPath = context.HttpContext.Request.Path;
                        //If there is an access token and if the request is going to signalR client
                        if (!string.IsNullOrEmpty(signalRToken) && requestPath.StartsWithSegments("/v1/live"))
                        {
                            var key = keyValueNamespace.Of(KeyVaultConstants.JwtSecretKey)?.Value;
                            var encryptionKey = keyValueNamespace.Of(KeyVaultConstants.JwtAccessTokenEncryptionKey)?.Value;
                            var token = signalRToken.ToString().Replace("%20", " ");
                            var jwtClient = context.Request.HttpContext.RequestServices.GetService<IJwtClient>();
                            var claims = jwtClient?.ValidateToken(token.Replace("Bearer ", string.Empty), key!, encryptionKey!, false) ?? [];
                            var enumerable = claims as Claim[] ?? claims.ToArray();
                            if (!enumerable.Any()) return Task.CompletedTask;
                            
                            context.Token = signalRToken.ToString().Replace("Bearer%20", string.Empty);
                            context.Request.Headers.Authorization = signalRToken;
                            context.HttpContext.User.AddIdentity(new ClaimsIdentity(enumerable));
                        }
                        
                        return Task.CompletedTask;
                    }
                };
            });
        
        return serviceCollection;
    }
    
    

    public static WebApplication MapSignalREndpoints(this WebApplication webApplication)
    {
        webApplication.MapHub<SnippetHub>("/v1/live/snippet");
        return webApplication;
    }
}