using FluentValidation;
using Sharecode.Backend.Utilities.Configuration;
using Sharecode.Backend.Utilities.Email;
using Sharecode.Backend.Utilities.KeyValue;

namespace Sharecode.Backend.Api.Extensions;

public static class ProgramExtensions
{
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

    public static IServiceCollection RegisterServices(this IServiceCollection service)
    {
        service.AddSingleton<IEmailClient, EmailClient>();
        return service;
    }

    public static IServiceCollection RegisterCors(this IServiceCollection service)
    {
        service.AddCors(options =>
        {
            options.AddPolicy("GatewayPolicy", builder =>
            {
                builder.WithOrigins("")
                    .WithMethods("PUT");
            });
        });
        return service;
    }
}