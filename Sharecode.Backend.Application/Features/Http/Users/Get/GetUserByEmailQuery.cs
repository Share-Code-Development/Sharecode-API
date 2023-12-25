using Sharecode.Backend.Application.Base;

namespace Sharecode.Backend.Application.Features.Http.Users.Get;

public record GetUserByEmailQuery(string EmailAddress, bool IncludeSettings = false) : IAppRequest<GetUserResponse>
{
}