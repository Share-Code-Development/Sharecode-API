using MediatR;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Sharecode.Backend.Application.Client;
using Sharecode.Backend.Application.Features.Snippet;
using Sharecode.Backend.Utilities.RedisCache;

namespace Sharecode.Backend.Api.Controller;

public class SnippetController(IAppCacheClient cache, IHttpClientContext requestContext, ILogger<AbstractBaseEndpoint> logger, IMediator mediator) : AbstractBaseEndpoint(cache, requestContext, logger, mediator)
{
    [HttpGet("{id}", Name = "View a snippet")]
    public async Task<IActionResult> GetSnippet(Guid id)
    {
        return Ok();
    }
    
    [HttpPost(Name = "Create a new snippet")]
    public async Task<IActionResult> CreateSnippet(Guid id)
    {
        var formCollection = await Request.ReadFormAsync();
        var file = formCollection.Files.FirstOrDefault();
        if (file == null)
        {
            return BadRequest("Missing file object");
        }
        var bodyRaw = formCollection["body"];

        if (string.IsNullOrEmpty(bodyRaw))
            return BadRequest("Invalid body object");

        var command = JsonConvert.DeserializeObject<CreateSnippetCommand>(bodyRaw.ToString());
        if (command == null)
            return BadRequest("Failed to parse the request");
        
        await using var ms = new MemoryStream();
        await file.CopyToAsync(ms);
        command.Content = ms.ToArray();

        var response = await mediator.Send(command);
        return CreatedAtRoute("Create a new snippet", response);
    }
}