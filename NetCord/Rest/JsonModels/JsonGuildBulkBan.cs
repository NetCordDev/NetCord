using System.Text.Json.Serialization;

namespace NetCord.Rest.JsonModels;

public class JsonGuildBulkBan
{
    [JsonPropertyName("banned_users")]
    public ulong[] BannedUsers { get; set; }

    [JsonPropertyName("failed_users")]
    public ulong[] FailedUsers { get; set; }
}
