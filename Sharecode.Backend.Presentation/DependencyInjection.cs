using Microsoft.Extensions.DependencyInjection;

namespace Sharecode.Backend.Presentation;

public static class DependencyInjection
{
    public static IServiceCollection RegisterPresentationLayer(this IServiceCollection collection)
    {
        return collection;
    }
}