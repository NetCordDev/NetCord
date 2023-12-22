using System.Text.Json.Serialization;

namespace NetCord.Rest;

public partial class RolePositionProperties
{
    [JsonPropertyName("id")]
    public ulong Id { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("position")]
    public int? Position { get; set; }

    public RolePositionProperties(ulong id)
    {
        Id = id;
    }
}
