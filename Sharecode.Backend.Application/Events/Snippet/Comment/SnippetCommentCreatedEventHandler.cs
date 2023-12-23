using System.Net;
using System.Text;
using MediatR;
using Microsoft.Extensions.Options;
using Sharecode.Backend.Application.Service;
using Sharecode.Backend.Domain.Events.Snippet.Comment;
using Sharecode.Backend.Domain.Repositories;
using Sharecode.Backend.Utilities.Configuration;
using Sharecode.Backend.Utilities.Email;
using Sharecode.Backend.Utilities.Extensions;

namespace Sharecode.Backend.Application.Events.Snippet.Comment;

public class SnippetCommentCreatedEventHandler(ISnippetCommentRepository snippetCommentRepository, IUserService service, IEmailClient emailClient, IOptions<FrontendConfiguration> frontEndConfiguration) : INotificationHandler<SnippetCommentCreateEvent>
{
    public async Task Handle(SnippetCommentCreateEvent notification, CancellationToken cancellationToken)
    {
        var snippetId = notification.SnippetId;
        var snippetComment = await snippetCommentRepository.GetAsync(notification.CommentId, includeSoftDeleted: false, track: true, token: cancellationToken);
        if(snippetComment == null)
            return;

        var mentionableUsers = snippetComment.Text.ExtractMentionableUsers();
        //Don't do anything if no users are in the comment's mention
        if(!mentionableUsers.Any())
            return;

        //Get all the users who have enabled notifications from the above list
        var users = await service.GetNotificationEnabledUsersAsync(mentionableUsers, cancellationToken);
        if(users.Any())
            return;
        
        var targets = new EmailTargets();
        foreach (var user in users)
        {
            targets.AddTarget(user.EmailAddress, user.FullName);
        }

        //var stringUrl = GenerateCommentUrl(frontEndConfiguration.Value.Base, snippetId, snippetComment.Id);
        var commentUrl = frontEndConfiguration.Value.CreateUrlFromBase(
            new Dictionary<string, string?>()
            {
                { "commentId", snippetComment.Id.ToString() }
            },
            "snippets", snippetId.ToString()
        );
        await emailClient.SendTemplateMailAsync(
            EmailTemplateKeys.MentionedInComment,
            targets,
            placeholders: new Dictionary<string, string>
            {
                { EmailPlaceholderKeys.SnippetCommentMessageAuthorKey, snippetComment.User.FullName },
                { EmailPlaceholderKeys.SnippetCommentMessageTextKey, ReplaceMentionsInComment(snippetComment.Text, users)},
                { EmailPlaceholderKeys.SnippetCommentMessageUrl, commentUrl } //TODO
            },
            subjectPlaceholders: new Dictionary<string, string>
            {
                { EmailPlaceholderKeys.SnippetCommentMessageAuthorKey, snippetComment.User.FullName }
            }
        );
    }

    private string ReplaceMentionsInComment(string messageText, List<Domain.Entity.Profile.User> users)
    {
        StringBuilder replacedText = new StringBuilder(messageText);
        foreach (var user in users)
        {
            replacedText.Replace($"<@{user.Id}>", $"<span class=\"username-mention\">@{user.FullName}</span>");
        }
        return replacedText.ToString();
    }
    
    private string GenerateCommentUrl(string baseUrl, Guid snippetId, Guid commentId)
    {
        // Encode parameters to ensure URL is valid and safe
        string safeSnippetId = WebUtility.UrlEncode(snippetId.ToString());
        string safeCommentId = WebUtility.UrlEncode(commentId.ToString());

        // Use string interpolation to insert these safe values into the URL
        return $"{baseUrl}/snippets/{safeSnippetId}?commentId={safeCommentId}";
    }
}