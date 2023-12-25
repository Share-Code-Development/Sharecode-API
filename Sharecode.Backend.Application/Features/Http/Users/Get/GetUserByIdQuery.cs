using Sharecode.Backend.Application.Base;

namespace Sharecode.Backend.Application.Features.Http.Users.Get;

/// <summary>
/// Get the user by Id
/// </summary>
/// <param name="UserId"></param>
/// <param name="IncludeSettings"></param>
public record GetUserByIdQuery(Guid UserId, bool IncludeSettings = false) : IAppRequest<GetUserResponse>
{
    
}