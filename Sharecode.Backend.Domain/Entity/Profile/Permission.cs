namespace Sharecode.Backend.Domain.Entity.Profile;

public record Permission(string Key, string Description, bool IsAdminOnly = false);

public static class Permissions
{
    public static Permission ViewDocument => new("view-document", "View all the documents");

    public static Permission AccessProfileOthers => new("read-profile-others", "Fetch/View others profile information");
}