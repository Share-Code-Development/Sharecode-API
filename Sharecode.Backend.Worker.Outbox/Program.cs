using System.Reflection;
using Microsoft.Extensions.Hosting;
using Quartz;
using Serilog;
using Sharecode.Backend.Utilities.KeyValue;
using Sharecode.Backend.Worker.Outbox;
using Sharecode.Backend.Worker.Outbox.Extensions;

var builder = Host.CreateApplicationBuilder(args);
SharecodeOutboxWorker.RegisterConverter();

var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true) // Use appsettings.json as the default
    .AddJsonFile("appsettings.Development.json", optional: true) // Try adding this line specifically
    .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
    .AddEnvironmentVariables()
    .Build();

builder.Services.AddSerilog(x => x.ReadFrom.Configuration(builder.Configuration));
builder.Services.BindConfigurationEntries(builder.Configuration);
builder.Services.AddSingleton<IKeyValueClient, KeyValueClient>();

//Cheeky way to access KeyValueClient
var keyValueClient = builder.Services.GetKeyValueClient();
var kvNameSpace = await keyValueClient.GetKeysOfNamespaceAsync();

if (kvNameSpace == null)
{
    throw new Exception("Failed to fetch the key value name space");
}
builder.Services.AddSingleton<Namespace>(kvNameSpace);
builder.Services.RegisterLayeredServices(kvNameSpace, builder);
builder.Services.RegisterFakeContextAccessor();
builder.Services.CreateShareCodeJobScheduler();
builder.Services.AddQuartzHostedService();

var host = builder.Build();


host.Run();