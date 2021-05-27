using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Slugify;

namespace Audiochan.Core.Extensions
{
    public static class StringExtensions
    {
        public static string GenerateSlug(this string phrase)
        {
            if (string.IsNullOrWhiteSpace(phrase)) return string.Empty;

            return new SlugHelper().GenerateSlug(phrase);
        }
        
        public static List<string> FormatTags(this IEnumerable<string> tags)
        {
            var formattedTags = tags
                .Where(t => !string.IsNullOrWhiteSpace(t))
                .Select(t => t.GenerateSlug())
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
    }
}