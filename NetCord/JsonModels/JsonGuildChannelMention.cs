using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public record JsonGuildChannelMention : JsonEntity
{
    [JsonPropertyName("guild_id")]
    public Snowflake GuildId { get; init; }

    [JsonPropertyName("type")]
    public ChannelType Type { get; init; }

    [JsonPropertyName("name")]
    public string Name { get; init; }
}
