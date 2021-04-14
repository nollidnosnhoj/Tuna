using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Audiochan.Core.Extensions
{
    public static class StringExtensions
    {
        public static bool ValidSlug(this string phrase)
        {
            return !string.IsNullOrWhiteSpace(phrase)
                   && Regex.IsMatch(phrase.ToLower(), @"/^[a-z0-9]+(?:-[a-z0-9]+)*$/");
        }

        public static string GenerateSlug(this string phrase)
        {
            if (string.IsNullOrWhiteSpace(phrase)) return string.Empty;

            // Convert foreign characters into ASCII
            var text = new IdnMapping().GetAscii(phrase);

            return text.GenerateTag();
        }

        public static string GenerateTag(this string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return string.Empty;

            var text = name.ToLower();

            //  Remove all invalid characters.  
            text = Regex.Replace(text, @"[^a-z0-9\s-]", "");

            //  Convert multiple spaces into one space
            text = Regex.Replace(text, @"\s+", " ").Trim();

            //  Replace spaces by underscores.
            text = Regex.Replace(text, @"\s", "-");

            // If there's consecutive hyphens, only use one.
            text = Regex.Replace(text, @"[-]{2,}", "-");

            // Dirty: Trim any hyphens
            text = text.Trim('-');

            return text;
        }

        public static List<string> FormatTags(this IEnumerable<string> tags)
        {
            // If a tag is null, return an empty string
            // If the tag is valid, do not tagify it
            // Remove any entries that are empty.
            var formattedTags = tags
                .Select(t => t ?? string.Empty)
                .Select(t => t.ValidSlug() ? t : t.GenerateTag())
                .Where(t => !string.IsNullOrWhiteSpace(t))
                .ToList();

            return formattedTags;
        }

        public static string ToSnakeCase(this string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }

            var startUnderscores = Regex.Match(input, @"^_+");
            return startUnderscores + Regex.Replace(input, @"([a-z0-9])([A-Z])", "$1_$2").ToLower();
        }

        public static string Truncate(this string input, int length)
        {
            var stringBuilder = new StringBuilder(length);
            stringBuilder.Append(input.Length > 30 ? input.Substring(0, length) : input);
            return stringBuilder.ToString();
        }
    }
}