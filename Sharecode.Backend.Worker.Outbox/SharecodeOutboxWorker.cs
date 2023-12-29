using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Sharecode.Backend.Domain.Base.Primitive;
using Sharecode.Backend.Domain.Entity.Profile;
using Sharecode.Backend.Infrastructure;
using Sharecode.Backend.Utilities.Configuration;

namespace Sharecode.Backend.Worker.Outbox;

internal static class SharecodeOutboxWorker
{
    public static Assembly[] ReferencingAssemblies => new[]
    {
        typeof(KeyValueConfiguration).Assembly,
        typeof(Program).Assembly,
        typeof(DependencyInjection).Assembly,
        typeof(Application.DependencyInjection).Assembly,
        typeof(BaseEntity).Assembly
    };

    public static JsonSerializerSettings JsonSerializerSettings => new JsonSerializerSettings()
    {
        ContractResolver = new CamelCasePropertyNamesContractResolver(),
        NullValueHandling = NullValueHandling.Ignore,
        DateFormatHandling = DateFormatHandling.IsoDateFormat,
    };

    public static void RegisterConverter()
    {
        JsonSerializerSettings.Converters.Add(new PermissionConverter());
        JsonConvert.DefaultSettings = () => SharecodeOutboxWorker.JsonSerializerSettings;
    }
}