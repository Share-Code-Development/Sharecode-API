using System.Net;
using Serilog;
using Sharecode.Backend.Api.Extensions;
using Sharecode.Backend.Application;
using Sharecode.Backend.Infrastructure;
using Sharecode.Backend.Presentation;
using Sharecode.Backend.Utilities.KeyValue;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

builder.Services.BindConfigurationEntries(builder.Configuration);
builder.Services.AddSingleton<IKeyValueClient, KeyValueClient>();

//Cheeky way to access KeyValueClient
ServiceProvider serviceProvider = builder.Services.BuildServiceProvider();
IKeyValueClient keyValueClient = serviceProvider.GetRequiredService<IKeyValueClient>();
Namespace? keyValueNamespace = await keyValueClient.GetKeysOfNamespaceAsync();

if (keyValueNamespace == null)
{
    throw new Exception("Failed to fetch the key value name space");
}

builder.Services.AddSingleton<Namespace>(keyValueNamespace);
builder.Services.RegisterApplicationLayer()
    .RegisterInfrastructureLayer(keyValueNamespace)
    .RegisterPresentationLayer();

builder.Host.UseSerilog((ctx, conf) =>
{
    conf.ReadFrom.Configuration(ctx.Configuration);
});

builder.Services.AddControllers();
builder.Services.RegisterServices();

/*
builder.Services.AddStackExchangeRedisCache(options =>
{
    var connectionStringTask = keyValueClient.GetAsync("redis-connection-string");
    var passwordTask = keyValueClient.GetAsync("redis-connection-password");
    var userTask = keyValueClient.GetAsync("redis-connection-user-name");
    KeyValue?[] values = Task.WhenAll(connectionStringTask, passwordTask, userTask).GetAwaiter().GetResult();
    if (values.Length != 3)
    {
        throw new Exception("Failed to fetch redis connection string");

    }
    options.Configuration = values[0]?.Value;
    string[]? strings = values[0]?.Value.Split(":");
    DnsEndPoint endPoint = new DnsEndPoint(strings[0], Convert.ToInt32(strings[1]));
    options.ConfigurationOptions = new ConfigurationOptions()
    {
        AbortOnConnectFail = false,
        User = values[2]?.Value,
        Password = values[1]?.Value,
        EndPoints = {endPoint}
    };
});
*/

var app = builder.Build();

app.UseSerilogRequestLogging();
app.UseForwardedHeaders();
app.UseHttpsRedirection();
app.UseRouting();
/*app.UseAuthentication();
app.UseAuthorization();*/
app.MapControllers();

app.Run();