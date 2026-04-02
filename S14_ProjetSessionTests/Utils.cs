using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace S14_ProjetSessionTests;

internal static class Utils
{
    public static string GetToken(string html)
    {
        var match = Regex.Match(
            html,
            @"<input[^>]*name=""__RequestVerificationToken""[^>]*value=""([^""]+)""",
            RegexOptions.IgnoreCase);

        return match.Success ? match.Groups[1].Value : "";
    }
}