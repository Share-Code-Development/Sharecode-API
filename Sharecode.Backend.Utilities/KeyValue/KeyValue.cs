namespace Sharecode.Backend.Utilities.KeyValue;

public class KeyValue
{
    public KeyValue(string namespaceIdentifier, string key, string value)
    {
        NamespaceIdentifier = namespaceIdentifier;
        Key = key;
        Value = value;
    }

    public readonly string NamespaceIdentifier;
    public readonly string Key;
    public readonly string Value;
}