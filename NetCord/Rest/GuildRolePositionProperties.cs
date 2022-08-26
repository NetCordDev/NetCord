using System.Text.Json.Serialization;

namespace NetCord.Rest;

public class GuildRolePositionProperties
{
    [JsonPropertyName("id")]
    public Snowflake Id { get; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("position")]
    public int? Position { get; set; }

    public GuildRolePositionProperties(Snowflake id)
    {
        Id = id;
    }
}
