using System.Text.Json.Serialization;

namespace NetCord.Rest;

internal class GuildBulkBanProperties(IEnumerable<ulong> userIds, int deleteMessageSeconds) : GuildBanProperties(deleteMessageSeconds)
{
    [JsonPropertyName("user_ids")]
    public IEnumerable<ulong> UserIds { get; set; } = userIds;
}
