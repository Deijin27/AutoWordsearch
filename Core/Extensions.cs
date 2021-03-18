using System.Text.RegularExpressions;

namespace Core
{
    public static class Extensitons 
    {
        // public static string RemoveNonAlphanumeric(this string text) => Regex.Replace(text, "[^a-zA-Z0-9 -]+", "");

        public static string RemoveWhitespace(this string text) => Regex.Replace(text, @"\s+", "");
    }
}
