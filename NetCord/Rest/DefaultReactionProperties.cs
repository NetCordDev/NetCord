using System.Text.Json.Serialization;

namespace NetCord.Rest;

public struct DefaultReactionProperties
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("emoji_id")]
    public Snowflake? Id { get; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("emoji_name")]
    public string? Unicode { get; }

    public DefaultReactionProperties(Snowflake id)
    {
        Id = id;
    }

    public DefaultReactionProperties(string unicode)
    {
        Unicode = unicode;
    }
}
