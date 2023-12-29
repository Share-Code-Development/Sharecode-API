using System.Collections.Concurrent;

namespace Sharecode.Backend.Api.Exceptions;

public class ExceptionCache
{
    public static readonly ConcurrentDictionary<string, string> Exceptions = new();
}