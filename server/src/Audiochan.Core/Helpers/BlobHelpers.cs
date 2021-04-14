using System;
using Audiochan.Core.Entities;

namespace Audiochan.Core.Helpers
{
    public static class BlobHelpers
    {
        public static string GetAudioBlobName(Audio audio)
        {
            return audio.UploadId + audio.FileExt;
        }

        public static string GetPictureBlobName(DateTime dateTime)
        {
            return $"{Guid.NewGuid():N}_{dateTime:yyyyMMddHHmmss}.jpg";
        }
    }
}