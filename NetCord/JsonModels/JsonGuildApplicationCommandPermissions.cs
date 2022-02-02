using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

internal record JsonGuildApplicationCommandPermissions
{
    [JsonPropertyName("id")]
    public DiscordId CommandId { get; init; }

    [JsonPropertyName("application_id")]
    public DiscordId ApplicationId { get; init; }

    [JsonPropertyName("guild_id")]
    public DiscordId GuildId { get; init; }

    [JsonPropertyName("permissions")]
    public JsonApplicationCommandPermission[] Permissions { get; init; }
}