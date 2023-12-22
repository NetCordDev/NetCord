using System.Text.Json.Serialization;

namespace NetCord.Rest;

internal class GuildBanProperties
{
    public GuildBanProperties(int deleteMessageSeconds)
    {
        DeleteMessageSeconds = deleteMessageSeconds;
    }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("delete_message_seconds")]
    public int DeleteMessageSeconds { get; set; }
}
