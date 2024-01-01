using Sharecode.Backend.Application.Base;

namespace Sharecode.Backend.Application.Features.Http.Users.Metadata.List;

public class ListUserMetadataQuery : IAppRequest<ListUserMetadataResponse?>
{
    public Guid UserId { get; init; }
    public HashSet<string> Queries { get; } = [];
}