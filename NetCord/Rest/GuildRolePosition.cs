using System.Text.Json.Serialization;

namespace NetCord.Rest;

public class GuildRolePosition
{
    [JsonPropertyName("id")]
    public Snowflake Id { get; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("position")]
    public int? Position { get; set; }

    public GuildRolePosition(Snowflake id)
    {
        Id = id;
    }
}