using System.Text.Json;
using System.Text.Json.Serialization;

namespace NetCord;

[GenerateMethodsForProperties]
public partial class EmojiProperties
{
    private EmojiProperties()
    {
    }

    /// <summary>
    /// Creates an <see cref="EmojiProperties"/> instance for a custom emoji.
    /// </summary>
    /// <param name="id">The ID of a custom emoji.</param>
    /// <returns></returns>
    public static EmojiProperties Custom(ulong id)
    {
        return new()
        {
            Id = id,
        };
    }

    /// <summary>
    /// Creates an <see cref="EmojiProperties"/> instance for a standard emoji.
    /// </summary>
    /// <param name="name">The unicode of a standard emoji.</param>
    /// <returns></returns>
    public static EmojiProperties Standard(string name)
    {
        return new()
        {
            Name = name,
        };
    }

    /// <summary>
    /// The ID of a custom emoji.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("id")]
    public ulong? Id { get; set; }

    /// <summary>
    /// The unicode of a standard emoji.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    public class GuildChannelEmojiPropertiesConverter : JsonConverter<EmojiProperties>
    {
        private static readonly JsonEncodedText _emojiId = JsonEncodedText.Encode("emoji_id");
        private static readonly JsonEncodedText _emojiName = JsonEncodedText.Encode("emoji_name");

        public override EmojiProperties? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => throw new NotImplementedException();

        public override void Write(Utf8JsonWriter writer, EmojiProperties value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            var id = value.Id;
            if (id.HasValue)
                writer.WriteNumber(_emojiId, id.GetValueOrDefault());
            else
                writer.WriteString(_emojiName, value.Name);

            writer.WriteEndObject();
        }
    }
}
