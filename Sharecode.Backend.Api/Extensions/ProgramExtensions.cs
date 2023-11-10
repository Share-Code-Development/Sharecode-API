using Sharecode.Backend.Utilities.KeyValue;

namespace Sharecode.Backend.Api.Extensions;

public static class ProgramExtensions
{
    public static IServiceCollection BindConfigurationEntries(this IServiceCollection service, ConfigurationManager configurationManager)
    {
        KeyValueConfiguration configuration = new KeyValueConfiguration();
        configurationManager.GetSection("CloudFlareKV").Bind(configuration);
        service.Configure<KeyValueConfiguration>(options => configurationManager.GetSection("CloudFlareKV").Bind(options));
        return service;
    }
}