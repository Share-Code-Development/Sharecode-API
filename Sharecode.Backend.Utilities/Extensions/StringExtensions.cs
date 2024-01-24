using System.Text.RegularExpressions;

namespace Sharecode.Backend.Utilities.Extensions;

public static class StringExtensions
{
    private static readonly Regex CamelCaseRegex = new Regex("(\\B[A-Z])", RegexOptions.Compiled);
    private static readonly Regex HyphenatedRegex = new Regex("(?<!^)([A-Z])", RegexOptions.Compiled);

    private static readonly Regex MentionRegex = new Regex(@"<@([0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12})>", RegexOptions.Compiled);

    public static string ToCapitalized(this string input)
    {
        return CamelCaseRegex.Replace(input, " $1");
    }

    public static string ToHyphenated(this string input)
    {
        return HyphenatedRegex.Replace(input, "-$1");
    }
    
    public static string ToHyphenatedLower(this string input)
    {
        return HyphenatedRegex.Replace(input, "-$1").ToLower();
    }
    
    public static string ToHyphenatedUpper(this string input)
    {
        return HyphenatedRegex.Replace(input, "-$1").ToUpper();
    }

    public static string CombineNonNulls(this string? input, string separator, params string?[] combinations)
    { 
        List<string?> nonNulls = new List<string?>();
        if(!string.IsNullOrWhiteSpace(input))
            nonNulls.Add(input);

        foreach(var s in combinations)
        {
            if(!string.IsNullOrWhiteSpace(s))
                nonNulls.Add(s);
        }
        return string.Join(separator, nonNulls);
    }

    public static HashSet<Guid> ExtractMentionableUsers(this string? text)
    {
        if (string.IsNullOrEmpty(text))
            return new HashSet<Guid>();

        HashSet<Guid> mentionableUsers = new();
        try
        {
            var matches = MentionRegex.Matches(text);
            foreach (Match match in matches)
            {
                var value = match.Groups[1].Value;
                if (Guid.TryParse(value, out Guid guid))
                {
                    mentionableUsers.Add(guid);
                }
            }
        }
        catch (Exception ignored)
        {
            // ignored
        }

        return mentionableUsers;
    }
    
    public static HashSet<Guid> ExtractMentionableUsers(this IEnumerable<string>? texts)
    {
        var mentionableUsers = new HashSet<Guid>();

        if (texts == null) return mentionableUsers;
        
        foreach (var text in texts)
        {
            if(string.IsNullOrEmpty(text))
                continue;
            
            var matches = MentionRegex.Matches(text);
            foreach (Match match in matches)
            {
                var value = match.Groups[1].Value;
                if (Guid.TryParse(value, out Guid guid))
                {
                    mentionableUsers.Add(guid);
                }
            }
        }

        return mentionableUsers;
    }
    
    

}