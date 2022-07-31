using System.Collections.Generic;
using Audiochan.Core;

namespace Audiochan.Tests.Common.Builders
{
    public static class MediaStorageSettingBuilder
    {
        public static List<string> ValidAudioTypes = new()
        {
            "audio/mp3",
            "audio/mpeg",
            "audio/ogg"
        };

        public static List<string> ValidImageTypes = new()
        {
            "image/jpeg"
        };

        public static long MaxAudioSize = 262_144_000;
        public static long MaxImageSize = 5_000_000;
        
        public static AudioStorageSettings BuildAudioDefault()
        {
            return new()
            {
                Bucket = "audiochan",
                TempBucket = "audiochan-temp",
                ValidContentTypes = ValidAudioTypes,
                MaximumFileSize = MaxAudioSize
            };
        }
        
        public static PictureStorageSettings BuildImageDefault()
        {
            return new()
            {
                Bucket = "audiochan",
                ValidContentTypes = ValidImageTypes,
                MaximumFileSize = MaxImageSize
            };
        }
    }
}