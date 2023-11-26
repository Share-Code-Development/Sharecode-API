using MediatR;
using Sharecode.Backend.Application.Service;
using Sharecode.Backend.Domain.Entity.Profile;
using Sharecode.Backend.Domain.Exceptions;

namespace Sharecode.Backend.Application.Features.Users.ForgotPassword;

public class ForgotPasswordCommandHandler(IUserService service) : IRequestHandler<ForgotPasswordCommand, bool>
{
    public async Task<bool> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        try
        {
            return await service.RequestForgotPassword(request.EmailAddress, token: cancellationToken);
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