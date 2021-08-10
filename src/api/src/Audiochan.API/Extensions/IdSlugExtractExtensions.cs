using Audiochan.Core.Common.Models;

namespace Audiochan.API.Extensions
{
    public static class IdSlugExtractExtensions
    {
        private static readonly (long, string) DefaultEmpty = (0, "");
        
        public static (long, string) ExtractIdAndSlugFromSlug(this string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return DefaultEmpty;
            
            // If the value is a bigint, we can assume that it's the id of the resource.
            if (long.TryParse(value, out var id))
            {
                return (id, "");
            }

            // Separate id and slug
            var splits = value.Split('-');
            var idString = splits[0];
            var slugString = string.Join('-', splits[1..]);

            // Invalidate if the first portion of the slug is not a bigint
            return !long.TryParse(idString, out id) 
                ? DefaultEmpty 
                : (id, slugString);
        }
    }
}