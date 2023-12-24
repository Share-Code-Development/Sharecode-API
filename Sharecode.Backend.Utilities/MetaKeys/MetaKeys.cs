namespace Sharecode.Backend.Utilities.MetaKeys;

public static class MetaKeys
{
    public static class SnippetKeys
    {
        public static MetaKey LimitComments => new("LimitComments", typeof(bool));
    }

    public static class UserKeys
    {
        public static MetaKey RecentlyVisitedSnippets => new("RecentSnippets", typeof(List<Guid>));
    }
}