using System.Text.Json;
using System.Text.Json.Serialization;

namespace NetCord.Rest;

public class GuildWelcomeScreenChannelProperties
{
    [JsonPropertyName("channel_id")]
    public Snowflake ChannelId { get; }

    [JsonPropertyName("description")]
    public string Description { get; set; }

    [JsonPropertyName("emoji_id")]
    public GuildWelcomeScreenChannelPropertiesEmoji? Emoji { get; set; }

    public GuildWelcomeScreenChannelProperties(Snowflake channelId, string description)
    {
        ChannelId = channelId;
        Description = description;
    }
}

[JsonConverter(typeof(GuildWelcomeScreenChannelPropertiesEmojiConverter))]
public class GuildWelcomeScreenChannelPropertiesEmoji
{
    public string? Unicode { get; }

    public Snowflake? EmojiId { get; }

    public GuildWelcomeScreenChannelPropertiesEmoji(string unicode)
    {
        Unicode = unicode;
    }

    public GuildWelcomeScreenChannelPropertiesEmoji(Snowflake emojiId)
    {
        EmojiId = emojiId;
    }

    private class GuildWelcomeScreenChannelPropertiesEmojiConverter : JsonConverter<GuildWelcomeScreenChannelPropertiesEmoji>
    {
        public override GuildWelcomeScreenChannelPropertiesEmoji? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => throw new NotImplementedException();
        public override void Write(Utf8JsonWriter writer, GuildWelcomeScreenChannelPropertiesEmoji value, JsonSerializerOptions options)
        {
            if (value.EmojiId != null)
                JsonSerializer.Serialize(writer, value.EmojiId, options);
            else
            {
                writer.WriteNullValue();
                writer.WriteString("emoji_name", value.Unicode);
            }
        }
    }
}