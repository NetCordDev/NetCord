using System.Text.Json.Serialization;

namespace NetCord.Rest;

public struct ForumGuildChannelDefaultReactionProperties
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("emoji_id")]
    public Snowflake? Id { get; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("emoji_name")]
    public string? Unicode { get; }

    public ForumGuildChannelDefaultReactionProperties(Snowflake id)
    {
        Id = id;
    }

    public ForumGuildChannelDefaultReactionProperties(string unicode)
    {
        Unicode = unicode;
    }
}
