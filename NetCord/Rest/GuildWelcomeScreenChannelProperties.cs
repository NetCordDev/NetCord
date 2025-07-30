using System.Text.Json.Serialization;

namespace NetCord.Rest;

public partial class GuildWelcomeScreenChannelProperties(ulong channelId, string description)
{
    [JsonPropertyName("channel_id")]
    public ulong ChannelId { get; set; } = channelId;

    [JsonPropertyName("description")]
    public string Description { get; set; } = description;

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("emoji_id")]
    public ulong? EmojiId { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("emoji_name")]
    public string? EmojiName { get; set; }
}
