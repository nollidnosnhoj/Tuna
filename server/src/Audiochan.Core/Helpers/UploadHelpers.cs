using System;

namespace Audiochan.Core.Helpers
{
    public static class UploadHelpers
    {
        public static string GenerateUploadId()
        {
            return Guid.NewGuid().ToString("N");
        }
    }
}