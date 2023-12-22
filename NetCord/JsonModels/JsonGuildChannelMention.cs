using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public class JsonGuildChannelMention : JsonEntity
{
    [JsonPropertyName("guild_id")]
    public ulong GuildId { get; set; }

    [JsonPropertyName("type")]
    public ChannelType Type { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }
}
