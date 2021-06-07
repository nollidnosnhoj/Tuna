using System.Collections.Generic;
using Audiochan.Core.Common.Settings;

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
        
        public static MediaStorageSettings.StorageSettings BuildAudioDefault()
        {
            return new()
            {
                Container = "audios",
                Bucket = "audiochan",
                ValidContentTypes = ValidAudioTypes,
                MaximumFileSize = MaxAudioSize
            };
        }
        
        public static MediaStorageSettings.StorageSettings BuildImageDefault()
        {
            return new()
            {
                Container = "images",
                Bucket = "audiochan",
                ValidContentTypes = ValidImageTypes,
                MaximumFileSize = MaxImageSize
            };
        }
    }
}