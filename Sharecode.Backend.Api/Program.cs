using MicroElements.Swashbuckle.FluentValidation.AspNetCore;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.OpenApi.Models;
using Prometheus;
using Serilog;
using Sharecode.Backend.Api.Extensions;
using Sharecode.Backend.Api.Middleware.Http;
using Sharecode.Backend.Infrastructure;
using Sharecode.Backend.Utilities.KeyValue;

var builder = WebApplication.CreateBuilder(args);



//Create a unique session to this runtime
var singletonSession = builder.CreateAndRegisterSession();

Sharecode.Backend.Api.SharecodeRestApi.RegisterConverter();

builder.Host.UseSerilog((ctx, conf) =>
{
    conf.ReadFrom.Configuration(ctx.Configuration);
});


builder.Services.BindConfigurationEntries(builder.Configuration);
//Adds KeyValueClient  Singleton
//Adds EmailClient - Singleton
//Adds FileClient - FileClient would be responsible to handle the files
//Enables HttpContextAccessor and Adds Scoped HttpClientContext
//Add Validators on the API Layer
//Register Controllers, Newtonsoft JSON etc
builder.Services.RegisterCoreServices(builder.Environment, builder.Configuration);

//Cheeky way to access KeyValueClient
var keyValueClient = builder.Services.GetKeyValueClient();
var kvNameSpace = await keyValueClient.GetKeysOfNamespaceAsync();

if (kvNameSpace == null)
{
    throw new Exception("Failed to fetch the key value name space");
}
builder.Services.AddSingleton<Namespace>(kvNameSpace);
//Register Services from other layers
builder.Services.RegisterLayeredServices(kvNameSpace, builder);
builder.Services.AddShareCodeRateLimiting();

builder.Services.RegisterCors();

builder.Services.AddFluentValidationRulesToSwagger();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
    // Additional Swagger configuration if needed
});
builder.Services.BuildAuthenticationSchema(builder.Configuration, kvNameSpace);
builder.Services.UseHttpClientMetrics();
//---------------------------------------------------------------------------------------------------------------------
var app = builder.Build();
app.InitializeGroupStateManagers();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
    });
}

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

app.UseCors("DeployedLink");
app.UseSerilogRequestLogging();
app.UseRateLimiter();
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseForwardedHeaders();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseRouting();
app.UseHttpMetrics(x =>
{
    x.ReduceStatusCodeCardinality();
});
app.MapMetrics("/v1/api/metrics");
app.UseAuthorization();
app.MapSignalREndpoints();
app.MapControllers();
app.Run();

public partial class Program {}
