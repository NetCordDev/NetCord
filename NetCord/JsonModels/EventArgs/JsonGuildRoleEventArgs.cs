using System.Text.Json.Serialization;

namespace NetCord.JsonModels.EventArgs;

internal record JsonGuildRoleEventArgs
{
    [JsonPropertyName("guild_id")]
    public DiscordId GuildId { get; init; }

    [JsonPropertyName("role")]
    public JsonGuildRole Role { get; init; }
}