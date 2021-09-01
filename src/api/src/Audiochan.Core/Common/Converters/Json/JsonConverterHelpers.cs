using System.Text.Json;

namespace Audiochan.Core.Common.Converters.Json
{
    public static class JsonConverterHelpers
    {
        public static string? ReadPathFromUrl(ref Utf8JsonReader reader, string template)
        {
            if (reader.TokenType != JsonTokenType.String) throw new JsonException();
            
            var stringValue = reader.GetString();
            if (stringValue is null || !stringValue.StartsWith(template)) throw new JsonException();
            var path = stringValue[template.Length..];
            return path;
        }

        public static void WritePathToUrl(string template, Utf8JsonWriter writer, string? value)
        {
            writer.WriteStringValue(template + value);
        }
    }
}