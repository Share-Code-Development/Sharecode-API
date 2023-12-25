using Sharecode.Backend.Application.Base;

namespace Sharecode.Backend.Application.Features.Http.Users.Delete;

public class DeleteUserCommand : IAppRequest<DeleteUserResponse>
{
    public Guid UserId { get; set; }
    public bool? SoftDelete { get; set; } = true;
}