using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Sharecode.Backend.Utilities.KeyValue;

namespace Sharecode.Backend.Api.Controller;

[Route("[controller]")]
[ApiController]
public class KeyController : ControllerBase
{
    public readonly IKeyValueClient ValueClient;

    public KeyController(IKeyValueClient valueClient)
    {
        ValueClient = valueClient;
    }

    [HttpGet]
    public async Task<IActionResult> Result()
    {
        Namespace? namespaceAsync = await ValueClient.GetKeysOfNamespaceAsync();
        Console.WriteLine(JsonConvert.SerializeObject(namespaceAsync));
        return Ok(namespaceAsync);
    }
}