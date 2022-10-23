using System.Text.Json;
using System.Text.Json.Serialization;

namespace NetCord.Rest;

public partial class GuildWelcomeScreenChannelProperties
{
    [JsonPropertyName("channel_id")]
    public ulong ChannelId { get; }

    [JsonPropertyName("description")]
    public string Description { get; set; }

    [JsonPropertyName("emoji_id")]
    public GuildWelcomeScreenChannelEmojiProperties? Emoji { get; set; }

    public GuildWelcomeScreenChannelProperties(ulong channelId, string description)
    {
        ChannelId = channelId;
        Description = description;
    }

    [JsonSerializable(typeof(GuildWelcomeScreenChannelProperties))]
    public partial class GuildWelcomeScreenChannelPropertiesSerializerContext : JsonSerializerContext
    {
        public static GuildWelcomeScreenChannelPropertiesSerializerContext WithOptions { get; } = new(new(ToObjectExtensions._options));
    }
}

[JsonConverter(typeof(GuildWelcomeScreenChannelEmojiPropertiesConverter))]
public partial class GuildWelcomeScreenChannelEmojiProperties
{
    public string? Unicode { get; }

    public ulong? EmojiId { get; }

    public GuildWelcomeScreenChannelEmojiProperties(string unicode)
    {
        Unicode = unicode;
    }

    public GuildWelcomeScreenChannelEmojiProperties(ulong emojiId)
    {
        EmojiId = emojiId;
    }

    internal class GuildWelcomeScreenChannelEmojiPropertiesConverter : JsonConverter<GuildWelcomeScreenChannelEmojiProperties>
    {
        public override GuildWelcomeScreenChannelEmojiProperties? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => throw new NotImplementedException();
        public override void Write(Utf8JsonWriter writer, GuildWelcomeScreenChannelEmojiProperties value, JsonSerializerOptions options)
        {
            if (value.EmojiId != null)
                writer.WriteStringValue(value.EmojiId.ToString());
            else
            {
                writer.WriteNullValue();
                writer.WriteString("emoji_name", value.Unicode);
            }
        }
    }

    [JsonSerializable(typeof(GuildWelcomeScreenChannelEmojiProperties))]
    public partial class GuildWelcomeScreenChannelEmojiPropertiesSerializerContext : JsonSerializerContext
    {
        public static GuildWelcomeScreenChannelEmojiPropertiesSerializerContext WithOptions { get; } = new(new(ToObjectExtensions._options));
    }
}
