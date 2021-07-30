using System;

namespace Audiochan.Core.Common.Helpers
{
    public static class EncodingHelpers
    {
        public static byte[] ConvertBase64ToBytes(string base64)
        {
            // Parse the base64 data
            if (base64.Contains("base64"))
                base64 = base64.Split("base64")[1].Trim(',');

            var bytes = Convert.FromBase64String(base64);
            return bytes;
        }
    }
}