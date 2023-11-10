using System.Runtime.Serialization;

namespace Sharecode.Backend.Utilities.Exceptions;

public class NoKeyValueConfiguration : Exception
{
    public NoKeyValueConfiguration(string key, string nameSpace, string account) : base($"No value has been associated with the {key} on namespace {nameSpace}-{account}")
    {
    }

}