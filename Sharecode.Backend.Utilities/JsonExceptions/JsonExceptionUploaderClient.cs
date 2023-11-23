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

    public void CollectErrors(Type collectOf)
    {
        foreach (Assembly assembly in _assemblies)
        {
            var list = assembly.GetTypes()
                .Where(x =>
                {
                    return !string.IsNullOrEmpty(x.Namespace) &&
                           x.IsSubclassOf(collectOf);
                }).ToList();

            foreach (var type in list)
            {
                string className = type.Name;
                if (_errors.ContainsKey(className))
                {
                    Console.WriteLine($"An existing error with the same class exists on {className}!");
                    continue;
                }
                
                
                
                /*_errors[className]*/
            }
        }
    }
}