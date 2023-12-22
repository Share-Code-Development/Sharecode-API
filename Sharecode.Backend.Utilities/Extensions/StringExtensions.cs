using System.Text.RegularExpressions;

namespace Sharecode.Backend.Utilities.Extensions;

public static class StringExtensions
{
    private static readonly Regex CamelCaseRegex = new Regex("(\\B[A-Z])", RegexOptions.Compiled);
    private static readonly Regex HyphenatedRegex = new Regex("(?<!^)([A-Z])", RegexOptions.Compiled);

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

}