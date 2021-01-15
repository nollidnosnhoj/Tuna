using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace Audiochan.Core.Common.Extensions
{
    public static class StringExtensions
    {
        public static bool ValidSlug(this string phrase)
        {
            return Regex.IsMatch(phrase.ToLower(), @"/^[a-z0-9]+(?:-[a-z0-9]+)*$/");
        }
        
        public static string GenerateSlug(this string phrase)
        {
            // Convert foreign characters into ASCII
            var text = new IdnMapping().GetAscii(phrase);

            return text.GenerateTag();
        }
        
        public static string GenerateTag(this string name)
        {
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

        public static List<string> FormatTags(this IEnumerable<string?> tags)
        {
            return tags
                .Where(t => t != null)
                .Select(t => t!.ValidSlug() ? t : t.GenerateTag())
                .ToList();
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