﻿using MediatR;
using Serilog;
using Sharecode.Backend.Application.Client;
using Sharecode.Backend.Application.Data;
using Sharecode.Backend.Application.Exceptions;
using Sharecode.Backend.Application.Exceptions.Snippet;
using Sharecode.Backend.Application.Service;
using Sharecode.Backend.Domain.Entity.Profile;
using Sharecode.Backend.Domain.Enums;
using Sharecode.Backend.Domain.Repositories;
using Sharecode.Backend.Utilities.RedisCache;

namespace Sharecode.Backend.Application.Features.Http.Snippet.Delete;

public class DeleteSnippetCommandHandler(ILogger logger, IHttpClientContext context, ISnippetService snippetService, ISnippetRepository snippetRepository, IUnitOfWork unitOfWork) : IRequestHandler<DeleteSnippetCommand, DeleteResponse>
{
    public async Task<DeleteResponse> Handle(DeleteSnippetCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var requestingUser = await context.GetUserIdentifierAsync();
            if (!requestingUser.HasValue)
                throw new NoSnippetAccessException(request.SnippetId);

            var snippet = await snippetRepository.GetAsync(request.SnippetId, false, cancellationToken);
            if(snippet == null)
                return DeleteResponse.NotFoundWithId(request.SnippetId);

            var snippetOwnerId = snippet.OwnerId;
            #region Permission Check

            bool canDelete = false;

            if (!snippetOwnerId.HasValue)
            {
                var hasAdminDeletePermission = await context.HasPermissionAsync(Permissions.DeleteSnippetOthers, cancellationToken);
                if (hasAdminDeletePermission)
                    canDelete = true;

                throw new NotEnoughPermissionException("Delete Anonymous Snippet");
            }

            if (requestingUser.Value == snippetOwnerId)
            {
                canDelete = true;
            }
            else
            {
                canDelete = await context.HasPermissionAnyAsync([Permissions.DeleteSnippetOthers], cancellationToken);
            }
            #endregion
        
            if (!canDelete)
                throw new NotEnoughPermissionException("Delete snippet");

            var deleteSnippet = await snippetService.DeleteSnippet(request.SnippetId, requestingUser.Value);
            await unitOfWork.CommitAsync(cancellationToken);

            #region Cache Keys To Clear
            //Clear the snippet
            context.AddCacheKeyToInvalidate(CacheModules.Snippet, request.SnippetId.ToString());
            //Clear the comments of the snippet
            context.AddCacheKeyToInvalidate(CacheModules.SnippetComment, request.SnippetId.ToString());
            //Clear the user's recent snippets, my snippets etc
            context.AddCacheKeyToInvalidate(CacheModules.UserSnippet, snippetOwnerId.Value.ToString());
            //Self reaction of the user of that snippet
            context.AddCacheKeyToInvalidate(CacheModules.SnippetUserReactions, request.SnippetId.ToString());
            //When deleting the usage of the user should be recalculated
            if (snippetOwnerId.HasValue)
            {
                context.AddCacheKeyToInvalidate(CacheModules.UserSnippetUsage, snippetOwnerId.Value.ToString());
            }
            if (snippetOwnerId.Value != requestingUser.Value)
            {
                //If the owner is different, delete my recent snippets of the owner too, (Deleting from admin level may be)
                context.AddCacheKeyToInvalidate(CacheModules.UserSnippet, requestingUser.Value.ToString());
            }

            #endregion
            return DeleteResponse.DeletedOf(typeof(Domain.Entity.Snippet.Snippet), request.SnippetId,
                requestingUser.Value);
        }
        catch (Exception e)
        {
            logger.Error(e, "An unknown error occured while deleting snippet {SnippetId} due to {Message}", request.SnippetId, e.Message);
            return DeleteResponse.Error(request.SnippetId, e.Message);
        }
    }
}