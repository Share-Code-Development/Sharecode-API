using System.Reflection;

namespace Sharecode.Backend.Utilities.JsonExceptions;

public class JsonExceptionUploaderClient
{
    private readonly Dictionary<string, JsonExceptionModel> _errors = new();

    private List<Assembly> _assemblies;

    private JsonExceptionUploaderClient(List<Assembly> assemblies)
    {
        _assemblies = assemblies;
    }

    public static JsonExceptionUploaderClient FromAssemblies(params Assembly[] assembly)
    {
        return new JsonExceptionUploaderClient(assembly.ToList());
    }

    public void CollectErrors(Type ofErrorType)
    {
        foreach (Assembly assembly in _assemblies)
        {
            var typesWithAttribute = assembly.GetTypes()
                .Where(type =>
                {
                    return !string.IsNullOrEmpty(type.Namespace) &&
                           type.IsSubclassOf(ofErrorType) &&
                           type.GetCustomAttributes(typeof(ExceptionDetail), false).Length > 0;
                })
                .ToList();


            foreach (var type in typesWithAttribute)
            {
                var exceptionDetailAttribute = (ExceptionDetail)type.GetCustomAttributes(typeof(ExceptionDetail), false)[0];

                long errorCode = exceptionDetailAttribute.ErrorCode;
                string errorDescription = exceptionDetailAttribute.ErrorDescription;

                // Now you can use errorCode and errorDescription as needed.
                Console.WriteLine($"Type {type.Name} has ErrorCode: {errorCode}, ErrorDescription: {errorDescription}");
            }
        }
    }
}