using System.Text.Json;
using System.Text.Json.Serialization;

namespace NetCord.Rest;

public class RestError(int code, string message, IRestErrorGroup? error)
{
    [JsonPropertyName("code")]
    public int Code { get; } = code;

    [JsonPropertyName("message")]
    public string Message { get; } = message;

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("errors")]
    public IRestErrorGroup? Error { get; } = error;
}

[JsonConverter(typeof(IRestErrorGroupConverter))]
public interface IRestErrorGroup
{
    private static readonly JsonEncodedText _errors = JsonEncodedText.Encode("_errors");

    public class IRestErrorGroupConverter : JsonConverter<IRestErrorGroup>
    {
        public override IRestErrorGroup? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            reader.Read();
            if (reader.ValueTextEquals("_errors"u8))
            {
                RestErrorDetailGroup group = new(JsonSerializer.Deserialize(ref reader, Serialization.Default.IReadOnlyListRestErrorDetail)!);

                reader.Read();

                while (reader.TokenType is not JsonTokenType.EndObject)
                {
                    reader.Read();
                    reader.Skip();
                    reader.Read();
                }

                return group;
            }

            Dictionary<string, IRestErrorGroup> errors = [];
            while (true)
            {
                if (reader.TokenType is JsonTokenType.EndObject)
                    break;

                var key = reader.GetString()!;
                reader.Read();
                errors.Add(key, JsonSerializer.Deserialize(ref reader, Serialization.Default.IRestErrorGroup)!);
                reader.Read();
            }

            return new RestErrorGroup(errors);
        }

        public override void Write(Utf8JsonWriter writer, IRestErrorGroup value, JsonSerializerOptions options)
        {
            switch (value)
            {
                case RestErrorGroup group:
                    JsonSerializer.Serialize(writer, group.Errors, Serialization.Default.IReadOnlyDictionaryStringIRestErrorGroup);
                    break;
                case RestErrorDetailGroup group:
                    writer.WriteStartObject();
                    writer.WritePropertyName(_errors);
                    JsonSerializer.Serialize(writer, group.Errors, Serialization.Default.IReadOnlyListRestErrorDetail);
                    writer.WriteEndObject();
                    break;
                default:
                    throw new InvalidOperationException($"Invalid {nameof(IRestErrorGroup)} value.");
            }
        }
    }
}

public class RestErrorGroup(IReadOnlyDictionary<string, IRestErrorGroup> errors) : IRestErrorGroup
{
    public IReadOnlyDictionary<string, IRestErrorGroup> Errors { get; } = errors;
}

public class RestErrorDetailGroup(IReadOnlyList<RestErrorDetail> errors) : IRestErrorGroup
{
    public IReadOnlyList<RestErrorDetail> Errors { get; } = errors;
}

public class RestErrorDetail(string code, string message)
{
    [JsonPropertyName("code")]
    public string Code { get; } = code;

    [JsonPropertyName("message")]
    public string Message { get; } = message;
}
