using Serilog;
using Sharecode.Backend.Application.Service;
using Sharecode.Backend.Domain.Entity.Snippet;
using Sharecode.Backend.Domain.Repositories;

namespace Sharecode.Backend.Infrastructure.Service;

public class SnippetCommentService(ISnippetCommentRepository commentRepository, ILogger logger) : ISnippetCommentService
{

}