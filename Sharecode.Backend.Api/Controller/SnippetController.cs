using System.Text;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Sharecode.Backend.Application.Client;
using Sharecode.Backend.Application.Features.Snippet;
using Sharecode.Backend.Application.Features.Snippet.Comments.Create;
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
    [HttpGet("{id}", Name = "Get a snippet")]
    public async Task<IActionResult> GetSnippet([FromRoute] Guid id)
    {
        var snippetQuery = new GetSnippetQuery()
        {
            SnippetId = id
        };
        
        FrameCacheKey("snippet", id.ToString());
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
    /// <returns>The IActionResult representing the result of the operation.</returns>
    [HttpPost(Name = "Create a new snippet")]
    public async Task<IActionResult> CreateSnippet()
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

        if (string.IsNullOrEmpty(command.PreviewCode))
        {
            string preview;
            preview = command.Content.Length > 1200 ? Encoding.Default.GetString(command.Content, 0, 1200) : Encoding.Default.GetString(command.Content);
            command.PreviewCode = preview;
        }
        
        var response = await mediator.Send(command);
        return CreatedAtAction("GetSnippet", new {id = response.SnippetId} , response);
    }

    [HttpGet("{snippetId}/comments",Name = "Get the comments of snippets")]
    public async Task<IActionResult> GetSnippetComments(Guid snippetId)
    {
        FrameCacheKey("snippet-comment", snippetId.ToString());
        return Ok();
    }

    [HttpPost("{snippetId}/comments", Name = "Create a comment for snippet")]
    [Authorize]
    public async Task<IActionResult> CreateSnippetComments([FromRoute]Guid snippetId, [FromBody] CreateSnippetCommentCommand command)
    {
        command.SnippetId = snippetId;
        var response = await mediator.Send(command);
        return CreatedAtAction("GetSnippetComments", new { snippetId = response.Id }, response);
    }
}