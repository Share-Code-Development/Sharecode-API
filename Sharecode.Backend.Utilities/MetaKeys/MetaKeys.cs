namespace Sharecode.Backend.Utilities.MetaKeys;

public static class MetaKeys
{
    public static class SnippetKeys
    {
        public static MetaKey LimitComments => new("LIMIT_COMMENTS", typeof(bool));
    }

    public static class UserKeys
    {
        public static MetaKey RecentlyVisitedSnippets => new("RECENT_SNIPPETS", typeof(List<Guid>));
    }
}