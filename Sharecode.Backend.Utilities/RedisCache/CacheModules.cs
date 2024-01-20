namespace Sharecode.Backend.Utilities.RedisCache;

public static class CacheModules
{
    public static string Snippet => "snippet";
    public static string User => "user";
    public static string UserSnippetUsage => "user_snippet_usage";
    public static string UserSnippet => "user_snippets";
    public static string UserMetadata => "user_metadata";
    public static string InternalUserPermission => "int_user_permission";
    public static string SnippetComment => "snippet_comment";
    public static string SnippetUserReactions => "snippet_user_reactions";
}