using FluentValidation;
using Sharecode.Backend.Utilities.Configuration;
using Sharecode.Backend.Utilities.KeyValue;

namespace Sharecode.Backend.Api.Extensions;

public static class ProgramExtensions
{
    public static IServiceCollection BindConfigurationEntries(this IServiceCollection service, ConfigurationManager configurationManager)
    {
        service.Configure<KeyValueConfiguration>(options => configurationManager.GetSection("CloudFlareKV").Bind(options));
        service.Configure<JwtConfiguration>(options => configurationManager.GetSection("JWT").Bind(options));
        return service;
    }

    public static IServiceCollection RegisterServices(this IServiceCollection service)
    {
        return service;
    }
}