using MediatR;
using Sharecode.Backend.Application.Data;
using Sharecode.Backend.Domain.Entity.Profile;
using Sharecode.Backend.Domain.Entity.Snippet;
using Sharecode.Backend.Domain.Events.Users;
using Sharecode.Backend.Domain.Repositories;

namespace Sharecode.Backend.Application.Events.User;

public class AccountDeleteEventHandler(
    IUserRepository userRepository, 
    ISnippetRepository snippetRepository, 
    ISnippetCommentRepository snippetCommentRepository,
    ISnippetLineCommentRepository snippetLineCommentRepository,
    IRefreshTokenRepository refreshTokenRepository,
    IUnitOfWork unitOfWork
    ) : INotificationHandler<DeleteUserDomainEvent>
{
    public async Task Handle(DeleteUserDomainEvent notification, CancellationToken cancellationToken)
    {
        if (notification.SoftDelete)
        {
            await refreshTokenRepository.DeleteBatchAsync(new EntitySpecification<UserRefreshToken>(x => x.IssuedFor == notification.RequestedBy), cancellationToken);
            await snippetLineCommentRepository.DeleteBatchAsync(new EntitySpecification<SnippetLineComment>(x => x.UserId == notification.UserId), cancellationToken);
            await snippetCommentRepository.DeleteBatchAsync(new EntitySpecification<SnippetComment>(x => x.UserId == notification.UserId), cancellationToken);
            await snippetRepository.DeleteBatchAsync(new EntitySpecification<Domain.Entity.Snippet.Snippet>(x => x.Id == notification.UserId), cancellationToken);
            await userRepository.DeleteBatchAsync(new EntitySpecification<Domain.Entity.Profile.User>(x => x.Id == notification.UserId), cancellationToken);
            await unitOfWork.CommitAsync(cancellationToken);
            return;
        }
        
        
    }
}