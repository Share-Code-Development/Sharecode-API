using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Sharecode.Backend.Application;

public static class DependencyInjection
{
    public static IServiceCollection RegisterApplicationLayer(this IServiceCollection collection)
    {
        var assembly = typeof(DependencyInjection).Assembly;
        collection.AddMediatR(conf =>
        {
            conf.RegisterServicesFromAssembly(assembly);
        });
        
        collection.AddValidatorsFromAssembly(assembly);

        return collection;
    }
}