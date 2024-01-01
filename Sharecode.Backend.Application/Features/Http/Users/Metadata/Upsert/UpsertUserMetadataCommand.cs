using Sharecode.Backend.Application.Base;

namespace Sharecode.Backend.Application.Features.Http.Users.Metadata.Upsert;

public class UpsertUserMetadataCommand : IAppRequest<UpsertUserMetadataResponse>
{
    public Guid UserId { get; set; }
    public Dictionary<string, object> MetaDictionary = new();
}