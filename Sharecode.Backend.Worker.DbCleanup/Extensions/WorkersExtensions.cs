using Npgsql;
using Quartz;
using Sharecode.Backend.Utilities;
using Sharecode.Backend.Utilities.Configuration;
using Sharecode.Backend.Utilities.KeyValue;
using Sharecode.Backend.Worker.DbCleanup.Application;
using Sharecode.Backend.Worker.DbCleanup.Infrastructure;

namespace Sharecode.Backend.Worker.DbCleanup.Extensions;

internal static class WorkersExtensions
{
    public static IServiceCollection BindConfigurationEntries(this IServiceCollection service, ConfigurationManager configurationManager)
    {
        service.Configure<KeyValueConfiguration>(options => 
            configurationManager.GetSection("CloudFlareKV").Bind(options));

        return service;
    }
    
    public static IKeyValueClient GetKeyValueClient(this IServiceCollection serviceCollection)
    {
        ServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();
        IKeyValueClient keyValueClient = serviceProvider.GetRequiredService<IKeyValueClient>();
        return keyValueClient;
    }

    public static IServiceCollection RegisterOutboxProcessorScheduler(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddQuartz(conf =>
        {
            /*var outboxJob = new JobKey(nameof(ProcessOutboxJob));
            conf.AddJob<ProcessOutboxJob>(outboxJob)
                .AddTrigger(trigger =>
                {
                    trigger.ForJob(jobKey: outboxJob)
                        .WithSimpleSchedule(schedule => schedule.WithIntervalInSeconds(30).RepeatForever());
                });*/
        });
        return serviceCollection;
    }

    public static IServiceCollection RegisterConnection(this IServiceCollection serviceCollection, Namespace nameSpace)
    {
        NpgsqlConnectionStringBuilder connectionStringBuilder = new NpgsqlConnectionStringBuilder()
        {
            Database = nameSpace.Of(KeyVaultConstants.PsDatabase)?.Value ?? string.Empty,
            Host = nameSpace.Of(KeyVaultConstants.PsHost)?.Value ?? string.Empty,
            Username = nameSpace.Of(KeyVaultConstants.PsUserName)?.Value ?? string.Empty,
            Password = nameSpace.Of(KeyVaultConstants.PsPassword)?.Value ?? string.Empty,
            Port = int.Parse(nameSpace.Of(KeyVaultConstants.PsPort)?.Value ?? "5432")
        };

        serviceCollection.AddSingleton<NpgsqlConnectionStringBuilder>(connectionStringBuilder);
        
        serviceCollection.AddTransient<IPgSqlDbContext, PgSqlDbContext>();
        return serviceCollection;
    }
}