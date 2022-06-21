using System.Text.Json.Serialization;

namespace NetCord.Rest;

public class GuildEmojiOptions
{
    internal GuildEmojiOptions()
    {
    }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("roles")]
    public IEnumerable<Snowflake>? AllowedRoles { get; set; }
}