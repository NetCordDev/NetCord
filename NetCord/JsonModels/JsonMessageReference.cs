using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

[JsonSourceGenerationOptions(DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
internal record JsonMessageReference
{
    [JsonPropertyName("message_id")]
    public DiscordId? MessageId { get; init; }

    [JsonPropertyName("channel_id")]
    public DiscordId? ChannelId { get; init; }

    [JsonPropertyName("guild_id")]
    public DiscordId? GuildId { get; init; }

    [JsonPropertyName("fail_if_not_exists")]
    public bool? FailIfNotExists { get; init; }
}