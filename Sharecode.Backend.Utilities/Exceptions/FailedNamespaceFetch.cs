namespace Sharecode.Backend.Utilities.Exceptions;

public class FailedNamespaceFetch : Exception
{
    public FailedNamespaceFetch(string namespaceIdentifier) : base($"Failed to fetch keys from identifier {namespaceIdentifier}")
    {
    }
}