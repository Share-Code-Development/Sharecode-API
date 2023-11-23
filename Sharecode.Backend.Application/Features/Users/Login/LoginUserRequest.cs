using Sharecode.Backend.Application.Base;
using Sharecode.Backend.Domain.Enums;

namespace Sharecode.Backend.Application.Features.Users.Login;

public record LoginUserRequest(
    string? EmailAddress,
    string? Password,
    string? IdToken,
    AuthorizationType Type
    ) : IAppRequest<LoginUserResponse>;