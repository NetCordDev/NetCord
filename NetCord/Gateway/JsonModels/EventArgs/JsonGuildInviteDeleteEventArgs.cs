using System.Text.Json.Serialization;

namespace NetCord.Gateway.JsonModels.EventArgs;

public class JsonGuildInviteDeleteEventArgs
{
    [JsonPropertyName("channel_id")]
    public ulong InviteChannelId { get; set; }

    [JsonPropertyName("guild_id")]
    public ulong? GuildId { get; set; }

    [JsonPropertyName("code")]
    public string InviteCode { get; set; }
}
