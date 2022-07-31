using HashidsNet;

namespace Audiochan.Core.Helpers
{
    public static class HashIdHelper
    {
        public static string EncodeLong(long input)
        {
            return new Hashids("audiochan_hash", 10).EncodeLong(input);
        }
        
        public static long DecodeLong(string hash)
        {
            var results = new Hashids("audiochan_hash", 10).DecodeLong(hash);
            return results.Length > 0 ? results[0] : -1;
        }
    }
}