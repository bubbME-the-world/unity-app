using System.Text.RegularExpressions;

public static class StringExtension
{
    private static readonly Regex sWhitespace = new Regex(@"\s+");
    public static string ReplaceWhitespace(this string input, string replacement)
    {
        return sWhitespace.Replace(input, replacement);
    }
}