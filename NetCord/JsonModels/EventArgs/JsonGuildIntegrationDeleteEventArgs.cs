using System.Text.Json.Serialization;

namespace NetCord.JsonModels.EventArgs;

internal record JsonGuildIntegrationDeleteEventArgs
{
    [JsonPropertyName("id")]
    public DiscordId IntegrationId { get; init; }

    [JsonPropertyName("guild_id")]
    public DiscordId GuildId { get; init; }

    [JsonPropertyName("application_id")]
    public DiscordId? ApplicationId { get; init; }
}