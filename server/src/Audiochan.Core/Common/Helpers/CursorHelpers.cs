using System;
using System.Text;

namespace Audiochan.Core.Common.Helpers
{
    public static class CursorHelpers
    {
        public static (DateTime? since, string id) DecodeCursor(string cursor)
        {
            DateTime? dateTime = null;
            var id = string.Empty;

            if (!string.IsNullOrWhiteSpace(cursor))
            {
                var decodedCursor = Encoding.UTF8.GetString(Convert.FromBase64String(cursor));
                var decodedCursorArray = decodedCursor.Split(':');
                if (decodedCursorArray.Length == 2)
                {
                    if (long.TryParse(decodedCursorArray[0], out var timestamp))
                    {
                        dateTime = new DateTime(timestamp);
                    }

                    id = decodedCursorArray[1];
                }
            }

            return (dateTime, id);
        }

        public static string EncodeCursor(DateTime dateTime, string id)
        {
            var timestamp = dateTime.Ticks;
            var rawCursor = string.Join(':', timestamp, id);
            var bytes = Encoding.UTF8.GetBytes(rawCursor);
            return Convert.ToBase64String(bytes);
        }
    }
}