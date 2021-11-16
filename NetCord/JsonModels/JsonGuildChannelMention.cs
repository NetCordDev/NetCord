using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

internal record JsonGuildChannelMention : JsonEntity
{
    [JsonPropertyName("guild_id")]
    public DiscordId GuildId { get; init; }

    [JsonPropertyName("type")]
    public ChannelType Type { get; init; }

    [JsonPropertyName("name")]
    public string Name { get; init; }
}
