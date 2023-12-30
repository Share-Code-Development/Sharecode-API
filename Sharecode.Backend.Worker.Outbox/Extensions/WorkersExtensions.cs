using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Quartz;
using Sharecode.Backend.Application;
using Sharecode.Backend.Application.Client;
using Sharecode.Backend.Infrastructure;
using Sharecode.Backend.Presentation;
using Sharecode.Backend.Utilities.Configuration;
using Sharecode.Backend.Utilities.KeyValue;
using Sharecode.Backend.Utilities.SecurityClient;
using Sharecode.Backend.Worker.Outbox.Client;
using Sharecode.Backend.Worker.Outbox.Jobs;
using DependencyInjection = Sharecode.Backend.Application.DependencyInjection;

namespace Sharecode.Backend.Worker.Outbox.Extensions;

public static class WorkersExtensions
{
    public static IServiceCollection BindConfigurationEntries(this IServiceCollection service, ConfigurationManager configurationManager)
    {
        service.Configure<KeyValueConfiguration>(options => 
            configurationManager.GetSection("CloudFlareKV").Bind(options));
        service.Configure<LocalDirectoryConfiguration>(options =>
            configurationManager.GetSection("LocalDirectory").Bind(options));
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
    
    public static IServiceCollection RegisterLayeredServices(this IServiceCollection serviceCollection, Namespace keyValueNamespace, HostApplicationBuilder builder)
    {
        serviceCollection.RegisterInfrastructureLayer(keyValueNamespace, builder.Configuration, builder.Environment.IsDevelopment())
            .RegisterNotificationHandler();
        
        serviceCollection.AddSingleton<ISecurityClient>(new SecurityClient());
        return serviceCollection;
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
            var outboxJob = new JobKey(nameof(ProcessOutboxJob));
            conf.AddJob<ProcessOutboxJob>(outboxJob)
                .AddTrigger(trigger =>
                {
                    trigger.ForJob(jobKey: outboxJob)
                        .WithSimpleSchedule(schedule => schedule.WithIntervalInSeconds(30).RepeatForever());
                });
        });
        return serviceCollection;
    }

    public static IServiceCollection RegisterFakeContextAccessor(this IServiceCollection serviceCollection)
    {
        var httpContext = new DefaultHttpContext();

        // Populate the HttpContext as needed, for example:
        httpContext.Request.Scheme = "http";
        httpContext.Request.Host = new HostString("localhost:5000");
        httpContext.Request.Path = "/somepath";
        httpContext.Request.Method = "GET";
        httpContext.TraceIdentifier = Guid.NewGuid().ToString();
        // etc...

        // Create a fake HttpContextAccessor
        var httpContextAccessor = new HttpContextAccessor
        {
            HttpContext = httpContext,
        };
        serviceCollection.AddSingleton<IHttpContextAccessor>(httpContextAccessor);
        serviceCollection.AddSingleton<IHttpClientContext>(new JobClientContext());
        return serviceCollection;
    }

    private static IServiceCollection RegisterNotificationHandler(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddMediatR(conf =>
        {
            conf.RegisterServicesFromAssembly(typeof(WorkersExtensions).Assembly);
        });
        return serviceCollection;
    }
}