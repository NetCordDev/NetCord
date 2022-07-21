using System.Text.Json.Serialization;

namespace NetCord.Rest;

public class GuildBanProperties
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("delete_message_seconds")]
    public int? DeleteMessageSeconds { get; set; }
}