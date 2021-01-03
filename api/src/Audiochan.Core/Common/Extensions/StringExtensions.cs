using System.Globalization;
using System.Text.RegularExpressions;

namespace Audiochan.Core.Common.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// Generate a slug from a phrase.
        /// This is different from taggify as it will transform unicode characters.
        /// </summary>
        /// <param name="phrase"></param>
        /// <returns></returns>
        public static string GenerateSlug(this string phrase)
        {
            // Convert foreign characters into ASCII
            var text = new IdnMapping().GetAscii(phrase);

            return text.GenerateTag();
        }

        /// <summary>
        /// Transform a phrase into tag format, by removing symbols and accents.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
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

            return text;
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