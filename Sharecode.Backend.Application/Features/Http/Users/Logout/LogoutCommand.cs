using Sharecode.Backend.Application.Base;

namespace Sharecode.Backend.Application.Features.Http.Users.Logout;

public class LogoutCommand(string accessToken) : IAppRequest
{
    public string AccessToken => accessToken;
}