using MediatR;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Sharecode.Backend.Application.Client;
using Sharecode.Backend.Application.Features.Snippet;
using Sharecode.Backend.Application.Features.Snippet.Create;
using Sharecode.Backend.Application.Features.Snippet.Get;
using Sharecode.Backend.Utilities.RedisCache;

namespace Sharecode.Backend.Api.Controller;

public class SnippetController(IAppCacheClient cache, IHttpClientContext requestContext, ILogger<AbstractBaseEndpoint> logger, IMediator mediator) : AbstractBaseEndpoint(cache, requestContext, logger, mediator)
{
    /// <summary>
    /// GetSnippet method retrieves a snippet by its ID.
    /// </summary> <param name="id">The ID of the snippet to retrieve.
    /// </param> <returns>The snippet with the specified ID, if found. Otherwise, a NotFound status is returned.</returns>
    /// /
    [HttpGet("{id}", Name = "View a snippet")]
    public async Task<IActionResult> GetSnippet([FromRoute] Guid id)
    {
        var snippetQuery = new GetSnippetQuery()
        {
            SnippetId = id
        };
        
        FrameCacheKey("snippet","by_id", id.ToString());
        var cacheValue = await ScanAsync<GetSnippetResponse>();
        if (cacheValue != null)
        {
            return Ok(cacheValue);
        }
        
        var snippetResponse = await mediator.Send(snippetQuery);
        if (snippetResponse == null)
            return NotFound();

        await StoreCacheAsync(snippetResponse);
        return Ok(snippetResponse);
    }

    /// <summary>
    /// Creates a new snippet with the given ID.
    /// </summary>
    /// <param name="id">The ID of the snippet.</param>
    /// <returns>The IActionResult representing the result of the operation.</returns>
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

    [HttpGet("{id}/comments",Name = "Get the comments of snippets")]
    public async Task<IActionResult> GetSnippetComments()
    {
        return Ok();
    }
}