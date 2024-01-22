using System.Reflection;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Sharecode.Backend.Application;
using Sharecode.Backend.Domain.Base.Primitive;
using Sharecode.Backend.Domain.Entity.Profile;
using Sharecode.Backend.Utilities.Configuration;

namespace Sharecode.Backend.Api;

internal static class SharecodeRestApi
{
    public static Assembly[] ReferencingAssemblies => [
    
        typeof(KeyValueConfiguration).Assembly,
        typeof(Program).Assembly,
        typeof(DependencyInjection).Assembly,
        typeof(Application.DependencyInjection).Assembly,
        typeof(Presentation.DependencyInjection).Assembly,
        typeof(BaseEntity).Assembly
    ];

    public static JsonSerializerSettings JsonSerializerSettings => new JsonSerializerSettings()
    {
        ContractResolver = new CamelCasePropertyNamesContractResolver(),
        NullValueHandling = NullValueHandling.Ignore,
        DateFormatHandling = DateFormatHandling.IsoDateFormat,
    };

    public static void RegisterConverter()
    {
        JsonSerializerSettings.Converters.Add(new PermissionConverter());
        JsonConvert.DefaultSettings = () => SharecodeRestApi.JsonSerializerSettings;
    }
}