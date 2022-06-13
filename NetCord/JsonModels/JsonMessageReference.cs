using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

[JsonSourceGenerationOptions(DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
public record JsonMessageReference
{
    [JsonPropertyName("message_id")]
    public Snowflake? MessageId { get; init; }

    [JsonPropertyName("channel_id")]
    public Snowflake? ChannelId { get; init; }

    [JsonPropertyName("guild_id")]
    public Snowflake? GuildId { get; init; }

    [JsonPropertyName("fail_if_not_exists")]
    public bool? FailIfNotExists { get; init; }
}