using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Sharecode.Backend.Utilities.Extensions;
using Sharecode.Backend.Utilities.JsonExceptions;

namespace Sharecode.Backend.Utilities.ExceptionDetail;

public class ExceptionDetailClient
{
    private readonly Dictionary<string, JsonExceptionModel> _errors = new();
    public IReadOnlyDictionary<string, JsonExceptionModel> Errors => _errors;

    private readonly List<Assembly> _assemblies;

    private ExceptionDetailClient(List<Assembly> assemblies)
    {
        _assemblies = assemblies;
    }

    public static ExceptionDetailClient FromAssemblies(params Assembly[] assembly)
    {
        return new ExceptionDetailClient(assembly.ToList());
    }

    public ExceptionDetailClient CollectErrors(Type throwable)
    {
        
        if(!throwable.IsSubclassOf(typeof(Exception)))
            return this;
        
        foreach (Assembly assembly in _assemblies)
        {
            var typesWithAttribute = assembly.GetTypes()
                .Where(type =>
                {
                    return !string.IsNullOrEmpty(type.Namespace) &&
                           type.IsSubclassOf(throwable) &&
                           type.GetCustomAttributes(typeof(JsonExceptions.ExceptionDetail), false).Length > 0;
                })
                .ToList();


            foreach (var type in typesWithAttribute)
            {
                var exceptionDetailAttribute = (JsonExceptions.ExceptionDetail)type.GetCustomAttributes(typeof(JsonExceptions.ExceptionDetail), false)[0];

                long errorCode = exceptionDetailAttribute.ErrorCode;
                string errorDescription = exceptionDetailAttribute.ErrorDescription;

                JsonExceptionModel detail = new JsonExceptionModel
                {
                    ErrorCode = errorCode,
                    ErrorDescription = errorDescription,
                    NormalizedClassName = type.Name.ToCapitalized().TrimEnd()
                };

                _errors[type.Name] = detail;
            }
        }

        return this;
    }

    public override string ToString()
    {
        return JsonConvert.SerializeObject(_errors, new JsonSerializerSettings()
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        });
    }
}