using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Sharecode.Backend.Application.Behaviours;

namespace Sharecode.Backend.Application;

public static class DependencyInjection
{
    public static IServiceCollection RegisterApplicationLayer(this IServiceCollection collection)
    {
        var assembly = typeof(DependencyInjection).Assembly;
        collection.AddValidatorsFromAssembly(assembly);
        collection.AddMediatR(conf =>
        {
            conf.RegisterServicesFromAssembly(assembly);
            conf.AddOpenBehavior(typeof(ValidationBehaviour<,>));
        });
        return collection;
    }
}