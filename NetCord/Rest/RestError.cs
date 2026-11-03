using System.Text.Json;
using System.Text.Json.Serialization;

namespace NetCord.Rest;

/// <summary>
/// Represents an error returned by the Discord REST API.
/// </summary>
/// <param name="code">The Discord API error code.</param>
/// <param name="message">The human-readable error message.</param>
/// <param name="error">Detailed information about the fields that caused the error, if provided.</param>
public sealed class RestError(int code, string message, IRestErrorGroup? error)
{
    /// <summary>
    /// Gets the Discord API error code.
    /// </summary>
    [JsonPropertyName("code")]
    public int Code { get; } = code;

    /// <summary>
    /// Gets the human-readable error message.
    /// </summary>
    [JsonPropertyName("message")]
    public string Message { get; } = message;

    /// <summary>
    /// Gets detailed information about the fields that caused the error, if provided.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("errors")]
    public IRestErrorGroup? Error { get; } = error;

    /// <inheritdoc/>
    public override string ToString() => JsonSerializer.Serialize(this, Serialization.Default.RestError);
}

/// <summary>
/// Represents detailed error information returned by the Discord REST API.
/// </summary>
[JsonConverter(typeof(IRestErrorGroupConverter))]
public interface IRestErrorGroup : IJsonSerializable<IRestErrorGroup>
{
    /// <summary>
    /// Converts detailed Discord REST API errors to and from JSON.
    /// </summary>
    public class IRestErrorGroupConverter : JsonConverter<IRestErrorGroup>
    {
        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public override void Write(Utf8JsonWriter writer, IRestErrorGroup value, JsonSerializerOptions options)
        {
            value.WriteTo(writer);
        }
    }
}

/// <summary>
/// Represents a nested group of REST API errors keyed by field name or array index.
/// </summary>
/// <param name="errors">The nested errors.</param>
public class RestErrorGroup(IReadOnlyDictionary<string, IRestErrorGroup> errors) : IRestErrorGroup
{
    /// <summary>
    /// Gets the nested errors keyed by field name or array index.
    /// </summary>
    public IReadOnlyDictionary<string, IRestErrorGroup> Errors { get; } = errors;

    void IJsonSerializable<IRestErrorGroup>.WriteTo(Utf8JsonWriter writer)
    {
        JsonSerializer.Serialize(writer, Errors, Serialization.Default.IReadOnlyDictionaryStringIRestErrorGroup);
    }
}

/// <summary>
/// Represents a group of error details associated with a field or request.
/// </summary>
/// <param name="errors">The error details.</param>
public class RestErrorDetailGroup(IReadOnlyList<RestErrorDetail> errors) : IRestErrorGroup
{
    private static readonly JsonEncodedText _errors = JsonEncodedText.Encode("_errors");

    /// <summary>
    /// Gets the error details.
    /// </summary>
    public IReadOnlyList<RestErrorDetail> Errors { get; } = errors;

    void IJsonSerializable<IRestErrorGroup>.WriteTo(Utf8JsonWriter writer)
    {
        writer.WriteStartObject();
        writer.WritePropertyName(_errors);
        JsonSerializer.Serialize(writer, Errors, Serialization.Default.IReadOnlyListRestErrorDetail);
        writer.WriteEndObject();
    }
}

/// <summary>
/// Represents an individual REST API error detail.
/// </summary>
/// <param name="code">The machine-readable error code.</param>
/// <param name="message">The human-readable error message.</param>
public class RestErrorDetail(string code, string message)
{
    /// <summary>
    /// Gets the machine-readable error code.
    /// </summary>
    [JsonPropertyName("code")]
    public string Code { get; } = code;

    /// <summary>
    /// Gets the human-readable error message.
    /// </summary>
    [JsonPropertyName("message")]
    public string Message { get; } = message;
}
