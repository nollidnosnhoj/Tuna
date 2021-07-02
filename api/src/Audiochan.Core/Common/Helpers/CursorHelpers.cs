using System;

namespace Audiochan.Core.Common.Helpers
{
    public static class CursorHelpers
    {
        public static string Encode(Guid id, DateTime since)
        {
            var decoded = $"{id:N}:{since.Ticks}";
            return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(decoded));
        }

        public static (Guid?, DateTime?) Decode(string? cursor)
        {
            if (string.IsNullOrEmpty(cursor)) return (null, null);
            
            // decode base64 string
            var decoded = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(cursor));
            
            var sections = decoded.Split(":");
            
            // should anticipate 2 items in array. If not, cursor is invalid.
            if (sections.Length != 2) return (null, null);
            
            // should be able to parse id
            if (!Guid.TryParse(sections[0], out var id))
                return (null, null);
            
            // should be able to parse unix epoch ticks
            if (!long.TryParse(sections[1], out var ticks))
                return (null, null);
            
            return (id, new DateTime(ticks));
        }
    }
}