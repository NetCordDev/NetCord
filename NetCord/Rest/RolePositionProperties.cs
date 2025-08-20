using System.Text.Json.Serialization;

namespace NetCord.Rest;

[GenerateMethodsForProperties]
public partial class RolePositionProperties(ulong id)
{
    [JsonPropertyName("id")]
    public ulong Id { get; set; } = id;

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("position")]
    public int? Position { get; set; }
}
