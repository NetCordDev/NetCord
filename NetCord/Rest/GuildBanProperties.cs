using System.Text.Json.Serialization;

namespace NetCord.Rest;

internal class GuildBanProperties(int deleteMessageSeconds)
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("delete_message_seconds")]
    public int DeleteMessageSeconds { get; set; } = deleteMessageSeconds;
}
