using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace S14_ProjetSessionTests;

internal static class Utils
{
    public static string GetToken(string Html)
    {
        string pattern = @"<input .*name=""__RequestVerificationToken"" .* value=""(.*)""";
        return Regex.Match(Html, pattern).Groups[1].Value;
    }
}