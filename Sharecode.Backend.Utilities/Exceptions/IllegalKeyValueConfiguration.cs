using System.Runtime.Serialization;

namespace Sharecode.Backend.Utilities.Exceptions;

public class IllegalKeyValueConfiguration : Exception
{
    public IllegalKeyValueConfiguration()
    {
    }

    protected IllegalKeyValueConfiguration(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    public IllegalKeyValueConfiguration(string? message) : base(message)
    {
    }

    public IllegalKeyValueConfiguration(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}