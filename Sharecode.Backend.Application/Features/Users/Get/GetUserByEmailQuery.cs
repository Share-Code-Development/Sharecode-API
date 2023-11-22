using Sharecode.Backend.Application.Base;
using Sharecode.Backend.Application.Client;

namespace Sharecode.Backend.Application.Features.Users.Get;

public record GetUserByEmailQuery(string EmailAddress, bool IncludeSettings = false) : IAppRequest<GetUserResponse>
{
}