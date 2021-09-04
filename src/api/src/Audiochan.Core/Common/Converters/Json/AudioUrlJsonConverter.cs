using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Audiochan.Core.Common.Helpers;

namespace Audiochan.Core.Common.Converters.Json
{
    public class AudioUrlJsonConverter : JsonConverter<string>
    {
        public override string? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return JsonConverterHelpers.ReadPathFromUrl(ref reader, MediaLinkInvariants.AudioStream);
        }

        public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
        {
            JsonConverterHelpers.WritePathToUrl(MediaLinkInvariants.AudioStream, writer, value);
        }
    }
}