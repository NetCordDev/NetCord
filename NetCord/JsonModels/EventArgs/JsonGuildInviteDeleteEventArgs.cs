using System.Text.Json.Serialization;

namespace NetCord.JsonModels.EventArgs;

internal record JsonGuildInviteDeleteEventArgs
{
    [JsonPropertyName("channel_id")]
    public DiscordId InviteChannelId { get; init; }

    [JsonPropertyName("guild_id")]
    public DiscordId? GuildId { get; init; }

    [JsonPropertyName("code")]
    public string InviteCode { get; init; }
}