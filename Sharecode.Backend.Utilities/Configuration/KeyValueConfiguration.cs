namespace Sharecode.Backend.Utilities.Configuration;

public class KeyValueConfiguration
{
    public KeyValueConfiguration(string accountIdentifier, string namespaceIdentifier, string? authorizationBearer = null, string? apiEmail = null, string? apiKey = null)
    {
        AuthorizationBearer = authorizationBearer;
        ApiEmail = apiEmail;
        ApiKey = apiKey;
        AccountIdentifier = accountIdentifier;
        NamespaceIdentifier = namespaceIdentifier;
    }

    public KeyValueConfiguration()
    {
    }

    public string AuthorizationBearer { get; set; }
    public string ApiEmail { get; set; }
    public string ApiKey { get; set; }
    public string AccountIdentifier { get; set; }
    public string NamespaceIdentifier { get; set; }
}