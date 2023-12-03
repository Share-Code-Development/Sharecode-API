using System.Text;
using System.Threading.RateLimiting;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Serialization;
using Quartz;
using Sharecode.Backend.Api.Filters;
using Sharecode.Backend.Api.Service;
using Sharecode.Backend.Application;
using Sharecode.Backend.Application.Client;
using Sharecode.Backend.Infrastructure;
using Sharecode.Backend.Infrastructure.Jobs;
using Sharecode.Backend.Presentation;
using Sharecode.Backend.Utilities;
using Sharecode.Backend.Utilities.Configuration;
using Sharecode.Backend.Utilities.Email;
using Sharecode.Backend.Utilities.KeyValue;

namespace Sharecode.Backend.Api.Extensions;

public static class BootstrapExtensions
{
    
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
        return service;
    }

    public static IServiceCollection RegisterCoreServices(this IServiceCollection service, IWebHostEnvironment environment)
    {
        service.AddSingleton<IKeyValueClient, KeyValueClient>();
        service.AddSingleton<IEmailClient, EmailClient>();
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
        
        return service;
    }

    public static IServiceCollection RegisterLayeredServices(this IServiceCollection serviceCollection, Namespace keyValueNamespace, IWebHostEnvironment environment)
    {
        serviceCollection.RegisterInfrastructureLayer(keyValueNamespace, environment.IsDevelopment())
            .RegisterApplicationLayer()
            .RegisterPresentationLayer();

        return serviceCollection;
    }

    public static IServiceCollection RegisterCors(this IServiceCollection service)
    {
        service.AddCors(options =>
        {
            options.AddPolicy("DeployedLink", builder =>
            {
                builder.WithOrigins("https://sharecodeapp.onrender.com/", "http://localhost:4000/")
                    .AllowAnyHeader()
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

    public static IServiceCollection CreateShareCodeJobScheduler(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddQuartz(conf =>
        {
            var outboxJob = new JobKey(nameof(ProcessOutboxJob));
            conf.AddJob<ProcessOutboxJob>(outboxJob)
                .AddTrigger(trigger =>
                {
                    trigger.ForJob(jobKey: outboxJob)
                        .WithSimpleSchedule(schedule => schedule.WithIntervalInSeconds(20).RepeatForever());
                });
        });
        return serviceCollection;
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
            });

        return serviceCollection;
    }
}