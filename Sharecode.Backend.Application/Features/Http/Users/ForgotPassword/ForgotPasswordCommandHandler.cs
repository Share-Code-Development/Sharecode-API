using MediatR;
using Sharecode.Backend.Application.Data;
using Sharecode.Backend.Application.Service;
using Sharecode.Backend.Domain.Entity.Profile;
using Sharecode.Backend.Domain.Exceptions;

namespace Sharecode.Backend.Application.Features.Http.Users.ForgotPassword;

public class ForgotPasswordCommandHandler(IUserService service, IUnitOfWork unitOfWork) : IRequestHandler<ForgotPasswordCommand, bool>
{
    public async Task<bool> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var willSendPassword = await service.RequestForgotPassword(request.EmailAddress, token: cancellationToken);
            await unitOfWork.CommitAsync(cancellationToken);
            return willSendPassword;
        }
        catch (Exception e)
        {
            if (e is EntityNotFoundException entityNotFoundException)
            {
                if (entityNotFoundException.EntityType == typeof(User))
                    return false;
            }

            throw;
        }
    }
}