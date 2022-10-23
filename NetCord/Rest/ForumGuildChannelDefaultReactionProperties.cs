using System.Text.Json.Serialization;

namespace NetCord.Rest;

public struct ForumGuildChannelDefaultReactionProperties
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("emoji_id")]
    public ulong? Id { get; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("emoji_name")]
    public string? Unicode { get; }

    public ForumGuildChannelDefaultReactionProperties(ulong id)
    {
        Id = id;
    }

    public ForumGuildChannelDefaultReactionProperties(string unicode)
    {
        Unicode = unicode;
    }
}
