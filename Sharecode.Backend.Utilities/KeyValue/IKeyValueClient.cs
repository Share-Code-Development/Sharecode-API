namespace Sharecode.Backend.Utilities.KeyValue;

public interface IKeyValueClient
{
    KeyValueConfiguration Configuration { get; protected set; }
    Task<Namespace?> GetKeysOfNamespaceAsync();
    Task<KeyValue?> GetAsync(string key);
    Namespace? GetOfflineNamespace();

}