namespace Sharecode.Backend.Infrastructure.Exceptions;

public class NoEntityFoundException : Exception
{
    public NoEntityFoundException(Guid? entityId) : base($"Entity not found!. Id: {entityId?.ToString() ?? string.Empty} ")
    {
    }
}