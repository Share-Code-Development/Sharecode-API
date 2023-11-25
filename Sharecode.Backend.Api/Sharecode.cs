using System.Reflection;
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
}