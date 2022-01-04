using System.Text.Json.Serialization;

namespace NetCord
{
    public class CurrentGuildUserProperties
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("nick")]
        public string? Nickname { get; set; }
    }
}