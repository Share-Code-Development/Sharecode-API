using System.Net;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sharecode.Backend.Utilities.Exceptions;

namespace Sharecode.Backend.Utilities.KeyValue;

public sealed class KeyValueClient : IKeyValueClient
{

    private const string BearerTokenHeader = "Authorization";
    private const string XAuthEmailHeader = "X-Auth-Email";
    private const string XAuthKeyHeader = "X-Auth-Key";
    private const string CloudFlareApi = @"https://api.cloudflare.com/client/v4/accounts";
    private readonly int _keyCount = 0;
    public KeyValueConfiguration Configuration { get; set; }

    private Namespace? _namespace;
    
    public KeyValueClient(IOptions<KeyValueConfiguration> configuration)
    {
        Configuration = configuration.Value;
        if (!VerifyConfiguration())
        {
            throw new IllegalKeyValueConfiguration("Please at-least provide a single cloudflare key-value access mechanism");
        }
        _keyCount =
            ($"{CloudFlareApi}/{Configuration.AccountIdentifier}/storage/kv/namespaces/{Configuration.NamespaceIdentifier}/values/")
            .Length;
    }

    public KeyValueClient(KeyValueConfiguration configuration)
    {
        Configuration = configuration;
        _keyCount =
            ($"{CloudFlareApi}/{Configuration.AccountIdentifier}/storage/kv/namespaces/{Configuration.NamespaceIdentifier}/values/")
            .Length;        
    }

    public async Task<Namespace?> GetKeysOfNamespaceAsync()
    {
        var keys = await GetKeysAsync();
        await GetValueInternal(keys);
        return _namespace;
    }
    

    public Namespace? GetOfflineNamespace()
    {
        return _namespace;
    }

    private async Task<List<string>> GetKeysAsync()
    {
        using HttpClient client = new HttpClient();
        ConfigureClient(client);

        var message = await client.GetAsync($"https://api.cloudflare.com/client/v4/accounts/{Configuration.AccountIdentifier}/storage/kv/namespaces/{Configuration.NamespaceIdentifier}/keys");
        if (!message.IsSuccessStatusCode)
        {
            throw new FailedNamespaceFetch(Configuration.NamespaceIdentifier);
        }

        string responseRaw = await message.Content.ReadAsStringAsync();
        dynamic? response = JsonConvert.DeserializeObject<dynamic>(responseRaw);
        if (response == null)
            return new List<string>();

        JArray? array = response.result as JArray;
        if (array == null)
            return new List<string>();
        List<string> keys = new List<string>();
        foreach (var jToken in array)
        {
            string? key = jToken["name"]?.ToString();
            if(string.IsNullOrEmpty(key))
                continue;
            
            keys.Add(key);
        }

        return keys;
    }
    

    public async Task<KeyValue?> GetAsync(string key)
    {
        if (_namespace != null)
        {
            KeyValue? value = _namespace.Of(key);
            if (value != null)
                return value;
        }

        var keyInternal = await GetValueInternal(key);
        if (keyInternal == null)
            return null;
        List<KeyValue> values = null;
        if (_namespace != null)
        {
            values = _namespace.KeyValues.ToList();
        }
        else
        {
            values = new List<KeyValue>();
        }
        values.Add(keyInternal);
        _namespace = new Namespace(values);
        return keyInternal;
    }

    private async Task<KeyValue?> GetValueInternal(string key)
    {
        KeyValue? value = null;
        using (HttpClient client = new HttpClient())
        {
            value = await Call($"{CloudFlareApi}/{Configuration.AccountIdentifier}/storage/kv/namespaces/{Configuration.NamespaceIdentifier}/values/{key}", client);
        }
        return value;
    }
    
    private async Task<List<KeyValue>> GetValueInternal(List<string> keys)
    {
        List<KeyValue> value = new List<KeyValue>();
        using (HttpClient client = new HttpClient())
        {
            ConfigureClient(client);
            foreach (string key in keys)
            {
                try
                {
                    KeyValue? keyValue = await Call($"{CloudFlareApi}/{Configuration.AccountIdentifier}/storage/kv/namespaces/{Configuration.NamespaceIdentifier}/values/{key}", client);
                    if(keyValue == null)
                        continue;
                    
                    value.Add(keyValue);
                }
                catch (Exception ignored)
                {
                    // ignored
                }
            }
        }
        
        
        List<KeyValue> nameSpaces = null;
        if (_namespace != null)
        {
            nameSpaces = _namespace.KeyValues.ToList();
        }
        else
        {
            nameSpaces = new List<KeyValue>();
        }
        nameSpaces.AddRange(value);
        _namespace = new Namespace(nameSpaces);
        return value;
    }

    private async Task<KeyValue?> Call(string url, HttpClient client)
    {
        KeyValue value = null;
        string key = url.Substring(_keyCount);
        var response = await client.GetAsync(url);
        if (!response.IsSuccessStatusCode)
        {
            string errorResponseRaw = await response.Content.ReadAsStringAsync();
            dynamic? errorResponse = JsonConvert.DeserializeObject<dynamic>(errorResponseRaw);
            if (errorResponse == null)
                return value;
            dynamic error = errorResponse.errors[0];
            if (error.code == 10009 || error.message == "get: 'key not found'")
            {
                
                throw new NoKeyValueConfiguration(key, Configuration.NamespaceIdentifier,
                    Configuration.AccountIdentifier);
            }
                
            return value;
        }

        string responseString = await response.Content.ReadAsStringAsync();
        value = new KeyValue(Configuration.NamespaceIdentifier, key, responseString);
        return value;
    }
    
    private bool VerifyConfiguration()
    {
        if (
            string.IsNullOrEmpty(Configuration.AuthorizationBearer) &&
            string.IsNullOrEmpty(Configuration.ApiEmail) &&
            string.IsNullOrEmpty(Configuration.ApiKey) &&
            string.IsNullOrEmpty(Configuration.AccountIdentifier) &&
            string.IsNullOrEmpty(Configuration.NamespaceIdentifier)
        )
        {
            return false;
        }
        return true;
    }

    private void ConfigureClient(HttpClient client)
    {
        if (!string.IsNullOrEmpty(Configuration.AuthorizationBearer))
        {
            client.DefaultRequestHeaders.TryAddWithoutValidation(BearerTokenHeader,
                $"Bearer {Configuration.AuthorizationBearer}");
        }

        if (!string.IsNullOrEmpty(Configuration.ApiEmail))
        {
            client.DefaultRequestHeaders.TryAddWithoutValidation(XAuthEmailHeader, Configuration.ApiEmail);
        }

        if (!string.IsNullOrEmpty(Configuration.ApiKey))
        {
            client.DefaultRequestHeaders.TryAddWithoutValidation(XAuthKeyHeader, Configuration.ApiKey);
        }
    }

}