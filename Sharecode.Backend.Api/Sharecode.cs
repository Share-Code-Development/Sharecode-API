using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Sharecode.Backend.Application;
using Sharecode.Backend.Domain.Base.Primitive;
using Sharecode.Backend.Utilities.Configuration;

namespace Sharecode.Backend.Api;

public static class Sharecode
{
    public static Assembly[] ReferencingAssemblies => new[]
    {
        typeof(KeyValueConfiguration).Assembly,
        typeof(Program).Assembly,
        typeof(DependencyInjection).Assembly,
        typeof(Application.DependencyInjection).Assembly,
        typeof(Presentation.DependencyInjection).Assembly,
        typeof(BaseEntity).Assembly
    };

    public static JsonSerializerSettings JsonSerializerSettings => new JsonSerializerSettings()
    {
        ContractResolver = new CamelCasePropertyNamesContractResolver(),
        NullValueHandling = NullValueHandling.Ignore,
        DateFormatHandling = DateFormatHandling.IsoDateFormat
    };
}