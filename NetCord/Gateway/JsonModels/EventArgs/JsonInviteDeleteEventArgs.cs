using System.Text.Json.Serialization;

namespace NetCord.Gateway.JsonModels.EventArgs;

public class JsonInviteDeleteEventArgs
{
    [JsonPropertyName("channel_id")]
    public required ulong InviteChannelId { get; set; }

    [JsonPropertyName("guild_id")]
    public ulong? GuildId { get; set; }

    [JsonPropertyName("code")]
    public required string InviteCode { get; set; }
}
