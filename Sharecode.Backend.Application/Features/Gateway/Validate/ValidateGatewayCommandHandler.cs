using System.Net;
using MediatR;
using Sharecode.Backend.Application.Data;
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

    public ValidateGatewayCommandHandler(IGatewayRepository gatewayRepository, IUserService userService, IUnitOfWork unitOfWork)
    {
        _gatewayRepository = gatewayRepository;
        _userService = userService;
        _unitOfWork = unitOfWork;
    }

    public async Task<ValidateGatewayCommandResponse> Handle(ValidateGatewayAppRequest appRequest, CancellationToken cancellationToken)
    {
        GatewayRequest? gatewayRequest = await _gatewayRepository.GetAsync(appRequest.GatewayId, token: cancellationToken);
        if (gatewayRequest == null)
        {
            return ValidateGatewayCommandResponse.NotFound;
        }

        if (gatewayRequest.IsDeleted || gatewayRequest.IsCompleted || !gatewayRequest.IsValid)
        {
            return ValidateGatewayCommandResponse.Invalid(gatewayRequest.IsCompleted ? "Already completed" : "Invalid Request");
        }

        if (DateTime.UtcNow > gatewayRequest.Expiry)
        {
            return ValidateGatewayCommandResponse.Expired;
        }

        GatewayRequestType requestType = gatewayRequest.RequestType;
        ValidateGatewayCommandResponse? response = null;
        if (requestType == GatewayRequestType.VerifyUserAccount)
        {
             response = await HandleVerifyUserRequestAsync(gatewayRequest, cancellationToken);
        }

        response ??= new ValidateGatewayCommandResponse(HttpStatusCode.InternalServerError, "Unknown handler");
        gatewayRequest.SetProcessed();
        await _unitOfWork.CommitAsync(cancellationToken);
        return response;
    }

    private async Task<ValidateGatewayCommandResponse> HandleVerifyUserRequestAsync(GatewayRequest gatewayRequest, CancellationToken token = default)
    {
        Guid sourceId = gatewayRequest.SourceId;
        try
        {
            var verifyUserAsync = await _userService.VerifyUserAsync(sourceId, token);
            if (!verifyUserAsync)
                return ValidateGatewayCommandResponse.AlreadyVerified;
            
            return ValidateGatewayCommandResponse.Verified;
        }
        catch (Exception e)
        {
            if (e is EntityNotFoundException notFoundException)
            {
                if(notFoundException?.EntityType == typeof(User))
                    return ValidateGatewayCommandResponse.UserNotFound;
            }

            throw;
        }
    }
}