﻿using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Audiochan.Core.Common.Helpers;

namespace Audiochan.Core.Common.Converters.Json
{
    public abstract class PictureUrlJsonConverter : JsonConverter<string?>
    {
        protected abstract string Url { get; }

        public override string? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return JsonConverterHelpers.ReadPathFromUrl(ref reader, Url);
        }

        public override void Write(Utf8JsonWriter writer, string? value, JsonSerializerOptions options)
        {
            JsonConverterHelpers.WritePathToUrl(Url, writer, value);
        }
    }
    
    public class AudioPictureUrlJsonConverter : PictureUrlJsonConverter
    {
        protected override string Url => MediaLinkConstants.AudioPicture;
    }
    
    public class PlaylistPictureUrlJsonConverter : PictureUrlJsonConverter
    {
        protected override string Url => MediaLinkConstants.PlaylistPicture;
    }
    
    public class UserPictureUrlJsonConverter : PictureUrlJsonConverter
    {
        protected override string Url => MediaLinkConstants.UserPicture;
    }
}