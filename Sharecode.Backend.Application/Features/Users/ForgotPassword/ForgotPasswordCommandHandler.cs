using MediatR;
using Sharecode.Backend.Application.Service;

namespace Sharecode.Backend.Application.Features.Users.ForgotPassword;

public class ForgotPasswordCommandHandler(IUserService service) : IRequestHandler<ForgotPasswordCommand, bool>
{
    public async Task<bool> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        return await service.RequestForgotPassword(request.EmailAddress, token: cancellationToken);
    }
}