using System.Text.RegularExpressions;

namespace Sharecode.Backend.Utilities.Extensions;

public static class StringExtensions
{
    private static readonly Regex CamelCaseRegex = new Regex("(\\B[A-Z])", RegexOptions.Compiled);

    public static string ToCapitalized(this string input)
    {
        return CamelCaseRegex.Replace(input, " $1");
    }
    
    
    
}