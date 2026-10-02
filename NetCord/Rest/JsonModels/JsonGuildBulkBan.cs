using System.Text.Json.Serialization;

namespace NetCord.Rest.JsonModels;

public class JsonGuildBulkBan
{
    [JsonPropertyName("banned_users")]
    public required ulong[] BannedUsers { get; set; }

    [JsonPropertyName("failed_users")]
    public required ulong[] FailedUsers { get; set; }
}
