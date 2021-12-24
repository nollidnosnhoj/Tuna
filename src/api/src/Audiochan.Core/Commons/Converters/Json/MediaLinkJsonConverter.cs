using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Audiochan.Core.Commons.Converters.Json
{
    public abstract class MediaLinkJsonConverter : JsonConverter<string?>
    {
        protected abstract string BaseUrl { get; }

        public override string? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var value = reader.GetString();
            if (string.IsNullOrEmpty(value)) return null;
            if (!value.StartsWith(BaseUrl)) throw new JsonException();
            return value[BaseUrl.Length..];
        }

        public override void Write(Utf8JsonWriter writer, string? value, JsonSerializerOptions options)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                writer.WriteNullValue();
                return;
            }
            
            writer.WriteStringValue(BaseUrl + value);
        }
    }
    
    public class AudioPictureJsonConverter : MediaLinkJsonConverter
    {
        protected override string BaseUrl => MediaLinkConstants.AUDIO_PICTURE;
    }

    public class UserPictureJsonConverter : MediaLinkJsonConverter
    {
        protected override string BaseUrl => MediaLinkConstants.USER_PICTURE;
    }

    public class AudioStreamLinkJsonConverter : JsonConverter<string>
    {
        public override string? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var value = reader.GetString();
            if (string.IsNullOrEmpty(value)) return null;
            if (!value.StartsWith(MediaLinkConstants.AUDIO_STREAM)) throw new JsonException();
            return value[MediaLinkConstants.AUDIO_STREAM.Length..];
        }

        public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                writer.WriteNullValue();
                return;
            }
            
            writer.WriteStringValue(MediaLinkConstants.AUDIO_STREAM + value);
        }
    }
}