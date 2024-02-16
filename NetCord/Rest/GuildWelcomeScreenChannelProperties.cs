using System.Text.Json;
using System.Text.Json.Serialization;

namespace NetCord.Rest;

public partial class GuildWelcomeScreenChannelProperties(ulong channelId, string description)
{
    [JsonPropertyName("channel_id")]
    public ulong ChannelId { get; set; } = channelId;

    [JsonPropertyName("description")]
    public string Description { get; set; } = description;

    [JsonPropertyName("emoji_id")]
    [JsonConverter(typeof(EmojiPropertiesConverter))]
    public EmojiProperties? Emoji { get; set; }

    public class EmojiPropertiesConverter : JsonConverter<EmojiProperties>
    {
        private static readonly JsonEncodedText _emojiName = JsonEncodedText.Encode("emoji_name");

        public override EmojiProperties? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => throw new NotImplementedException();

        public override void Write(Utf8JsonWriter writer, EmojiProperties value, JsonSerializerOptions options)
        {
            var id = value.Id;
            if (id.HasValue)
                writer.WriteNumberValue(id.GetValueOrDefault());
            else
            {
                writer.WriteNullValue();
                writer.WriteString(_emojiName, value.Unicode);
            }
        }
    }
}
