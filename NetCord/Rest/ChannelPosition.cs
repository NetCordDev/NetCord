using System.Text.Json.Serialization;

namespace NetCord;

public class ChannelPosition : GuildRolePosition
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("lock_permissions")]
    public bool? LockPermissions { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("parent_id")]
    public DiscordId? CategoryId { get; set; }

    public ChannelPosition(DiscordId id) : base(id)
    {
    }
}