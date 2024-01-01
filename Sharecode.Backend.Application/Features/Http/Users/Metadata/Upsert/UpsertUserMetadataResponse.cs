namespace Sharecode.Backend.Application.Features.Http.Users.Metadata.Upsert;

public class UpsertUserMetadataResponse
{
    public Dictionary<string, object?> OldValues = new();

    private UpsertUserMetadataResponse(Dictionary<string, object?> oldValues)
    {
        OldValues = oldValues;
    }

    private UpsertUserMetadataResponse()
    {
    }

    public static UpsertUserMetadataResponse From(Dictionary<string, object?> oldValues)
    {
        return new UpsertUserMetadataResponse(oldValues);
    }

    public static UpsertUserMetadataResponse Empty => new();
}