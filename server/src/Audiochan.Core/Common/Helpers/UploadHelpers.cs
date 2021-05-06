using System;
using System.Threading.Tasks;

namespace Audiochan.Core.Common.Helpers
{
    public static class UploadHelpers
    {
        public static async Task<string> GenerateUploadId()
        {
            return await Nanoid.Nanoid.GenerateAsync("0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ", 21);
        }
    }
}