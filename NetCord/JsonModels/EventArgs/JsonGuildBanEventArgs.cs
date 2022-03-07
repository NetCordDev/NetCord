using System.Text.Json.Serialization;

namespace NetCord.JsonModels.EventArgs;

internal record JsonGuildBanEventArgs
{
    [JsonPropertyName("guild_id")]
    public DiscordId GuildId { get; init; }

    [JsonPropertyName("user")]
    public JsonUser User { get; init; }
}
