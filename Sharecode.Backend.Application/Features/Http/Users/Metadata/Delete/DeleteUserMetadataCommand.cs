using Sharecode.Backend.Application.Base;

namespace Sharecode.Backend.Application.Features.Http.Users.Metadata.Delete;

public class DeleteUserMetadataCommand : IAppRequest
{
    public Guid UserId { get; set; }
    public HashSet<string> Keys { get; } = [];
}