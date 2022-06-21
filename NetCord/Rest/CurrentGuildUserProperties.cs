using System.Text.Json.Serialization;

namespace NetCord.Rest;

public class CurrentGuildUserProperties
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("nick")]
    public string? Nickname { get; set; }
}