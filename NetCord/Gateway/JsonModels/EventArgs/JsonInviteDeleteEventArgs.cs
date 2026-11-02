using System.Text.Json.Serialization;

using JsonGuard;

namespace NetCord.Gateway.JsonModels.EventArgs;

[JsonGuard]
public partial class JsonInviteDeleteEventArgs
{
    [JsonPropertyName("channel_id")]
    public ulong InviteChannelId { get; set; }

    [JsonPropertyName("guild_id")]
    public ulong? GuildId { get; set; }

    [JsonPropertyName("code")]
    public string InviteCode { get; set; }
}
