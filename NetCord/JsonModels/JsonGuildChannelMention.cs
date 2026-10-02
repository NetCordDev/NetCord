using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public class JsonGuildChannelMention : JsonEntity
{
    [JsonPropertyName("guild_id")]
    public required ulong GuildId { get; set; }

    [JsonPropertyName("type")]
    public required ChannelType Type { get; set; }

    [JsonPropertyName("name")]
    public required string Name { get; set; }
}
