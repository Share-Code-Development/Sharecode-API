using Serilog;
using Sharecode.Backend.Api.Extensions;
using Sharecode.Backend.Application;
using Sharecode.Backend.Infrastructure;
using Sharecode.Backend.Presentation;
using Sharecode.Backend.Utilities.KeyValue;

var builder = WebApplication.CreateBuilder(args);

builder.Services.RegisterApplicationLayer()
    .RegisterInfrastructureLayer()
    .RegisterPresentationLayer();

builder.Host.UseSerilog((ctx, conf) =>
{
    conf.ReadFrom.Configuration(ctx.Configuration);
});

builder.Services.AddControllers();
builder.Services.BindConfigurationEntries(builder.Configuration);
builder.Services.AddScoped<IKeyValueClient, KeyValueClient>();

var app = builder.Build();

app.UseSerilogRequestLogging();
app.UseForwardedHeaders();
app.UseHttpsRedirection();
app.UseRouting();
/*app.UseAuthentication();
app.UseAuthorization();*/
app.MapControllers();

app.Run();