using System.Text.Json.Serialization;

namespace NetCord.Rest;

public class ChannelPositionProperties
{
    [JsonPropertyName("id")]
    public Snowflake Id { get; }

    [JsonPropertyName("position")]
    public int? Position { get; set; }

    [JsonPropertyName("lock_permissions")]
    public bool? LockPermissions { get; set; }

    [JsonPropertyName("parent_id")]
    public Snowflake? ParentId { get; set; }

    public ChannelPositionProperties(Snowflake id)
    {
        Id = id;
    }
}
