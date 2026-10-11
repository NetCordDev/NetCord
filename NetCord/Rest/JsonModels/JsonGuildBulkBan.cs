using System.Text.Json.Serialization;

using JsonGuard;

namespace NetCord.Rest.JsonModels;

[JsonGuard]
public partial class JsonGuildBulkBan
{
    [JsonPropertyName("banned_users")]
    public ulong[] BannedUsers { get; set; }

    [JsonPropertyName("failed_users")]
    public ulong[] FailedUsers { get; set; }
}
