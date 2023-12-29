namespace Sharecode.Backend.Utilities.MetaKeys;

public static class MetaKeys
{
    public static class SnippetKeys
    {
        public static MetaKey LimitComments => new("limitComments", typeof(bool));
    }

    public static class UserKeys
    {
        public static MetaKey RecentlyVisitedSnippets => new("recentSnippets", typeof(List<Guid>));
    }
}