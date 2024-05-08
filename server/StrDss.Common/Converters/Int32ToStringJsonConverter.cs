using System.Text.Json;
using System.Text.Json.Serialization;

namespace StrDss.Common.Converters
{
    public class Int32ToStringJsonConverter : JsonConverter<string>
    {
        public override string Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return reader.TokenType switch
            {
                JsonTokenType.String => reader.GetString() ?? "",
                JsonTokenType.Number => reader.TryGetInt32(out int result) ? $"{result}" : string.Empty,
                _ => string.Empty,
            };
        }

        public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value);
        }
    }
}
