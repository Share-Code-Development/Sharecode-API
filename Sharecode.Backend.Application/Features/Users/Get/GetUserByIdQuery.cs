using MediatR;
using Sharecode.Backend.Application.Base;

namespace Sharecode.Backend.Application.Features.Users.Get;

public record GetUserByIdQuery(Guid UserId, bool IncludeSettings = false) : IAppRequest<GetUserResponse>
{
    
}