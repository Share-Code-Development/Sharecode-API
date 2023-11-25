using Sharecode.Backend.Application.Base;

namespace Sharecode.Backend.Application.Features.Refresh.Get;

public record GetRefreshTokenCommand(string RefreshToken) : IAppRequest<GetRefreshTokenResponse>;