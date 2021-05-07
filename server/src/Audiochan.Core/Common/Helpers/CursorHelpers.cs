using System;
using System.Text;
using NodaTime;

namespace Audiochan.Core.Common.Helpers
{
    public static class CursorHelpers
    {
        public static (Instant? since, string id) DecodeCursor(string cursor)
        {
            Instant? dateTime = null;
            var id = string.Empty;

            if (!string.IsNullOrWhiteSpace(cursor))
            {
                var decodedCursor = Encoding.UTF8.GetString(Convert.FromBase64String(cursor));
                var decodedCursorArray = decodedCursor.Split(':');
                if (decodedCursorArray.Length == 2)
                {
                    if (long.TryParse(decodedCursorArray[0], out var timestamp))
                    {
                        dateTime = Instant.FromUnixTimeTicks(timestamp);
                    }

                    id = decodedCursorArray[1];
                }
            }

            return (dateTime, id);
        }

        public static string EncodeCursor(Instant dateTime, string id)
        {
            var timestamp = dateTime.ToUnixTimeTicks();
            var rawCursor = string.Join(':', timestamp, id);
            var bytes = Encoding.UTF8.GetBytes(rawCursor);
            return Convert.ToBase64String(bytes);
        }
    }
}