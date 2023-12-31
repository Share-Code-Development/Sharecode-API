using System.Net.Mime;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Sharecode.Backend.Domain.Base.Primitive;
using Sharecode.Backend.Infrastructure;
using Sharecode.Backend.Utilities.Configuration;

namespace Sharecode.Backend.Worker.DbCleanup;

public class SharecodeDbCleanupWorker
{
    public static Assembly[] ReferencingAssemblies => new[]
    {
        typeof(KeyValueConfiguration).Assembly,
        typeof(Program).Assembly,
        typeof(DependencyInjection).Assembly,
        typeof(BaseEntity).Assembly
    };

    public static JsonSerializerSettings JsonSerializerSettings => new JsonSerializerSettings()
    {
        ContractResolver = new CamelCasePropertyNamesContractResolver(),
        NullValueHandling = NullValueHandling.Ignore,
        DateFormatHandling = DateFormatHandling.IsoDateFormat,
    };
}