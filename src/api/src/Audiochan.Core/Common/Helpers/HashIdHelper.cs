using HashidsNet;

namespace Audiochan.Core.Common.Helpers
{
    public static class HashIdHelper
    {
        public static string EncodeLong(long input, string salt = "Audiochan", int minimumLength = 10)
        {
            return new Hashids(salt, minimumLength).EncodeLong(input);
        }
        
        public static long DecodeLong(string hash, string salt = "Audiochan", int minimumLength = 10)
        {
            var results = new Hashids(salt, minimumLength).DecodeLong(hash);
            return results.Length > 0 ? results[0] : -1;
        }
    }
}