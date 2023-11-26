using System.Net;
using MediatR;
using Sharecode.Backend.Application.Data;
using Sharecode.Backend.Application.Exceptions;
using Sharecode.Backend.Application.Service;
using Sharecode.Backend.Domain.Entity.Gateway;
using Sharecode.Backend.Domain.Entity.Profile;
using Sharecode.Backend.Domain.Enums;
using Sharecode.Backend.Domain.Exceptions;
using Sharecode.Backend.Domain.Repositories;

namespace Sharecode.Backend.Application.Features.Gateway.Validate;

public class ValidateGatewayCommandHandler : IRequestHandler<ValidateGatewayAppRequest, ValidateGatewayCommandResponse>
{
    private readonly IGatewayRepository _gatewayRepository;
    private readonly IUserService _userService;
    private readonly IUnitOfWork _unitOfWork;

    public ValidateGatewayCommandHandler(IGatewayRepository gatewayRepository, IUserService userService, IUnitOfWork unitOfWork, IUserRepository userRepository)
    {
        _gatewayRepository = gatewayRepository;
        _userService = userService;
        _unitOfWork = unitOfWork;
    }

    public async Task<ValidateGatewayCommandResponse> Handle(ValidateGatewayAppRequest appRequest, CancellationToken cancellationToken)
    {
        GatewayRequest? gatewayRequest =
            await _gatewayRepository.GetAsync(appRequest.GatewayId, token: cancellationToken);
        if (gatewayRequest == null)
        {
            return ValidateGatewayCommandResponse.NotFound;
        }
        
        try
        {


            if (gatewayRequest.IsDeleted || gatewayRequest.IsCompleted || !gatewayRequest.IsValid)
            {
                return ValidateGatewayCommandResponse.Invalid(gatewayRequest.IsCompleted
                    ? "Already completed"
                    : "Invalid Request");
            }

            if (DateTime.UtcNow > gatewayRequest.Expiry)
            {
                return ValidateGatewayCommandResponse.Expired;
            }

            GatewayRequestType requestType = gatewayRequest.RequestType;
            ValidateGatewayCommandResponse? response = requestType switch
            {
                GatewayRequestType.VerifyUserAccount => await HandleVerifyUserRequestAsync(gatewayRequest,
                    cancellationToken),
                GatewayRequestType.ForgotPassword => await HandleForgotPasswordRequestAsync(gatewayRequest, appRequest,
                    cancellationToken),
                _ => new ValidateGatewayCommandResponse(HttpStatusCode.InternalServerError, "Unknown handler")
            };
            return response;
        }
        finally
        {
            gatewayRequest.SetProcessed();
            await _unitOfWork.CommitAsync(cancellationToken);
        }

    }

    private async Task<ValidateGatewayCommandResponse> HandleForgotPasswordRequestAsync(GatewayRequest gatewayRequest, ValidateGatewayAppRequest appRequest, CancellationToken token = default)
    {
        var userId = gatewayRequest.SourceId;
        try
        {
            bool success = await _userService.ResetPasswordAsync(userId, appRequest.NewPassword, token);
            if (success)
                return ValidateGatewayCommandResponse.Success;

            return ValidateGatewayCommandResponse.Invalid("An unknown error occured");
        }
        catch (Exception e)
        {
            switch (e)
            {
                case EntityNotFoundException notFoundException when notFoundException?.EntityType == typeof(User):
                    return ValidateGatewayCommandResponse.UserNotFound;
                case AccountTemporarilySuspendedException:
                    return ValidateGatewayCommandResponse.AccountSuspended;
                default:
                    throw;
            }
        }
    }

    private async Task<ValidateGatewayCommandResponse> HandleVerifyUserRequestAsync(GatewayRequest gatewayRequest, CancellationToken token = default)
    {
        Guid sourceId = gatewayRequest.SourceId;
        try
        {
            var verifyUserAsync = await _userService.VerifyUserAsync(sourceId, token);
            if (!verifyUserAsync)
                return ValidateGatewayCommandResponse.AlreadyVerified;
            
            return ValidateGatewayCommandResponse.Success;
        }
        catch (Exception e)
        {
            switch (e)
            {
                case EntityNotFoundException notFoundException when notFoundException?.EntityType == typeof(User):
                    return ValidateGatewayCommandResponse.UserNotFound;
                case AccountTemporarilySuspendedException:
                    return ValidateGatewayCommandResponse.AccountSuspended;
                default:
                    throw;
            }
        }
    }
}