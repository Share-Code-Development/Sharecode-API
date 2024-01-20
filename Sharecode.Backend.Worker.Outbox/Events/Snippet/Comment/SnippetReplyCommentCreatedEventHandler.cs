using System.Text;
using MediatR;
using Microsoft.Extensions.Options;
using Sharecode.Backend.Application.Service;
using Sharecode.Backend.Domain.Events.Snippet.Comment;
using Sharecode.Backend.Domain.Repositories;
using Sharecode.Backend.Utilities.Configuration;
using Sharecode.Backend.Utilities.Email;
using Sharecode.Backend.Utilities.Extensions;
using Sharecode.Backend.Utilities.Extensions.Task;
using ILogger = Serilog.ILogger;

namespace Sharecode.Backend.Worker.Outbox.Events.Snippet.Comment;

public class SnippetReplyCommentCreatedEventHandler(ISnippetCommentRepository snippetCommentRepository, ISnippetCommentService commentService, IUserRepository userRepository, IEmailClient emailClient, IOptions<FrontendConfiguration> frontEndConfiguration, ILogger logger) : INotificationHandler<SnippetReplyCommentCreateEvent>
{
    public async Task Handle(SnippetReplyCommentCreateEvent notification, CancellationToken cancellationToken)
    {
        var childCommentWithParent = await snippetCommentRepository.GetChildCommentWithParent(notification.CommentId, false, false, cancellationToken);
        if(childCommentWithParent == null)
            return;

        var mentionedUserGuidFromChild = childCommentWithParent.Text.ExtractMentionableUsers();
        var mentionedUserGuidFromParent = childCommentWithParent.ParentComment?.Text.ExtractMentionableUsers() ?? [];

        var combinedUsers = new HashSet<Guid>(mentionedUserGuidFromChild);
        combinedUsers.UnionWith(mentionedUserGuidFromParent);
        //This is a hashset so duplicates won't be there, 
        //2 emails should be sent, one to the owner and one to all the mentioned users.
        //So if the parent comments owner's guid is in the hashset, it won't be readded.
        //If not, it will also fetch the users notification setting. So to send the reply notification
        // we don't need to send an extra request.
        
        //The owner won't get 2 emails, as the #SendMentionedEmailNotification will remove the owner of the
        //parent comment even if its mentioned.
        combinedUsers.Add(notification.Owner);
        combinedUsers.Add(childCommentWithParent.ParentComment!.UserId);
        var totalUsers = await userRepository.GetUsersForMentionWithNotificationSettings(combinedUsers, cancellationToken);
        if(!totalUsers.Any())
            return;
        
        var mentionedUsersFromChild = totalUsers.Where(x => mentionedUserGuidFromChild.Contains(x.Id)).ToList();

        var notificationEnabledUsersFromChild = mentionedUsersFromChild.Where(x => x.AccountSetting.EnableNotificationsForMentions).ToList();
        var parentCommentOwnerUser = totalUsers.FirstOrDefault(x => x.Id == (childCommentWithParent.ParentComment?.UserId ?? Guid.Empty));
        bool isParentCommentOwnerNotificationEnabled = parentCommentOwnerUser?.AccountSetting.EnableNotificationsForMentions ?? false;
        var childCommentOwnerUser = totalUsers.FirstOrDefault(x => x.Id == notification.Owner);
        
        var parentMessage = ReplaceMentionsInComment(childCommentWithParent.ParentComment?.Text ?? string.Empty, totalUsers);
        var childMessage = ReplaceMentionsInComment(childCommentWithParent.Text, totalUsers);
        
        var commentUrl = frontEndConfiguration.Value.CreateUrlFromBase(
            new Dictionary<string, string?>()
            {
                { "commentId", childCommentWithParent.ParentCommentId.ToString() },
                { "childId", childCommentWithParent.Id.ToString() }
            },
            "snippets", notification.SnippetId.ToString()
        );

        var taskReplyAlertForOwner = SendReplyAlertForOwner(isParentCommentOwnerNotificationEnabled ? parentCommentOwnerUser : null, parentMessage, childMessage,
            commentUrl, childCommentOwnerUser?.FullName ?? "Unknown");

        //Owners doesn't need to get the mention email, he has the above email
        var usersToSendMentions = new List<Domain.Entity.Profile.User>(notificationEnabledUsersFromChild);
        if (parentCommentOwnerUser != null)
        {
            usersToSendMentions.Remove(parentCommentOwnerUser);
        }

        var taskSendMention = SendMentionNotification(usersToSendMentions, parentMessage, childMessage, commentUrl, childCommentOwnerUser?.FullName ?? "Unknown");
        var (ownerAlert, mentionNotification) = await (taskReplyAlertForOwner, taskSendMention);

        logger.Information("SnippetReplyCommentCreated has been processed. SnippetId: {SnippetId}, CommentId: {CommentId}, TaskOwnerAlert: {OwnerAlert}, TaskMention: {MentionTask}"
            , notification.SnippetId, notification.CommentId, ownerAlert, mentionNotification);
    }

    private async Task<bool> SendReplyAlertForOwner(Domain.Entity.Profile.User? ownerUser, string formattedParentMessage, string formattedChildMessage, string formattedChildCommentUrl, string childCommentOwner)
    {
        try
        {
            if (ownerUser == null)
                return false;

            EmailTargets targets = new EmailTargets();
            targets.AddTarget(ownerUser.EmailAddress, ownerUser.FullName);
            
            var templatePlaceholders = new Dictionary<string, string>()
            {
                { EmailPlaceholderKeys.SnippetCommentMessageUrl, formattedChildCommentUrl },
                { EmailPlaceholderKeys.SnippetCommentChildMessageTextKey, formattedChildMessage },
                { EmailPlaceholderKeys.SnippetCommentMessageTextKey, formattedParentMessage },
                { EmailPlaceholderKeys.SnippetCommentMessageAuthorKey, childCommentOwner }
            };

            var subjectPlaceholder = new Dictionary<string, string>()
            {
                { EmailPlaceholderKeys.SnippetCommentMessageAuthorKey, childCommentOwner }
            };

            await emailClient.SendTemplateMailAsync(
                EmailTemplateKeys.RepliesInComment,
                targets,
                templatePlaceholders,
                subjectPlaceholder
            );
            return true;
        }
        catch (Exception e)
        {
            logger.Error(e, "Failed to send reply[Alert] notification mail for {Users} due to {Message}",
                ownerUser?.Id, e.Message);
            return false;
        }
    }

    private async Task<bool> SendMentionNotification(List<Domain.Entity.Profile.User> usersToSend, string formattedParentMessage, string formattedChildMessage, string formattedChildCommentUrl, string ownerFullName)
    {
        try
        {
            if(!usersToSend.Any())
                return false;
        
            EmailTargets targets = new EmailTargets();
            foreach (var user in usersToSend)
            {
                targets.AddTarget(user.EmailAddress, user.FullName);
            }

            var templatePlaceholders = new Dictionary<string, string>()
            {
                { EmailPlaceholderKeys.SnippetCommentMessageUrl, formattedChildCommentUrl },
                { EmailPlaceholderKeys.SnippetCommentChildMessageTextKey, formattedChildMessage },
                { EmailPlaceholderKeys.SnippetCommentMessageAuthorKey, ownerFullName }
            };

            var subjectPlaceholder = new Dictionary<string, string>()
            {
                { EmailPlaceholderKeys.SnippetCommentMessageAuthorKey, ownerFullName }
            };

            await emailClient.SendTemplateMailAsync(
                EmailTemplateKeys.MentionedInComment,
                targets,
                templatePlaceholders,
                subjectPlaceholder
            );
            return true;
        }
        catch (Exception e)
        {
            logger.Error(e, "Failed to send reply[Mention] notification mail for {Users} due to {Message}",
                string.Join(", ", usersToSend.Select(x => x.Id)), e.Message);
            return false;
        }
    }

    private string ReplaceMentionsInComment(string messageText, List<Domain.Entity.Profile.User> users)
    {
        if (string.IsNullOrEmpty(messageText))
            return string.Empty;
        
        StringBuilder replacedText = new StringBuilder(messageText);
        foreach (var user in users)
        {
            replacedText.Replace($"<@{user.Id}>", $"<span class=\"username-mention\">@{user.FullName}</span>");
        }
        return replacedText.ToString();
    }
}