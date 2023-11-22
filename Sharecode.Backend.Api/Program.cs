using System.Net;
using System.Text;
using FluentValidation;
using MicroElements.Swashbuckle.FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Quartz;
using Serilog;
using Sharecode.Backend.Api.Extensions;
using Sharecode.Backend.Api.Filters;
using Sharecode.Backend.Api.Middleware;
using Sharecode.Backend.Api.Service;
using Sharecode.Backend.Application;
using Sharecode.Backend.Application.Client;
using Sharecode.Backend.Infrastructure;
using Sharecode.Backend.Infrastructure.Jobs;
using Sharecode.Backend.Presentation;
using Sharecode.Backend.Utilities;
using Sharecode.Backend.Utilities.KeyValue;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

builder.Services.BindConfigurationEntries(builder.Configuration);
builder.Services.AddSingleton<IKeyValueClient, KeyValueClient>();
builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<IHttpClientContext, HttpClientContext>();

//Cheeky way to access KeyValueClient
ServiceProvider serviceProvider = builder.Services.BuildServiceProvider();
IKeyValueClient keyValueClient = serviceProvider.GetRequiredService<IKeyValueClient>();
Namespace? keyValueNamespace = await keyValueClient.GetKeysOfNamespaceAsync();

if (keyValueNamespace == null)
{
    throw new Exception("Failed to fetch the key value name space");
}

builder.Services.AddSingleton<Namespace>(keyValueNamespace);

//Register validators in this domain first
builder.Services.AddValidatorsFromAssembly(typeof(ProgramExtensions).Assembly);

builder.Services
    .RegisterInfrastructureLayer(keyValueNamespace)
    .RegisterApplicationLayer()
    .RegisterPresentationLayer();

builder.Host.UseSerilog((ctx, conf) =>
{
    conf.ReadFrom.Configuration(ctx.Configuration);
});

builder.Services.AddControllers(options =>
    {
        options.Filters.Add(new ApiRequestFilter());
    })
    .AddNewtonsoftJson(jsonOptions =>
    {
        jsonOptions.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
    });

builder.Services.RegisterServices();
builder.Services.AddQuartz(conf =>
{
    var outboxJob = new JobKey(nameof(ProcessOutboxJob));

    conf
        .AddJob<ProcessOutboxJob>(outboxJob)
        .AddTrigger(trigger =>
        {
            trigger.ForJob(jobKey: outboxJob)
                .WithSimpleSchedule(schedule => schedule.WithIntervalInSeconds(20).RepeatForever());
        });
});
IdentityModelEventSource.ShowPII = true;
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(bearerOptions =>
    {
        var value = keyValueNamespace.Of(KeyVaultConstants.JwtSecretKey)?.Value;
        if (value != null)
            bearerOptions.TokenValidationParameters = new TokenValidationParameters()
            {
                ValidateIssuerSigningKey = true,
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidIssuer = builder.Configuration["JWT:Issuer"],
                ValidAudience = builder.Configuration["JWT:Audience"],
                IssuerSigningKey =
                    new SymmetricSecurityKey(
                        Encoding.ASCII.GetBytes(value))
            };
    });

builder.Services.AddQuartzHostedService();
builder.Services.AddFluentValidationRulesToSwagger();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
    // Additional Swagger configuration if needed
});
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

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
    });}

app.UseSerilogRequestLogging();
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseForwardedHeaders();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseRouting();
app.UseAuthorization();
app.MapControllers();

app.Run();