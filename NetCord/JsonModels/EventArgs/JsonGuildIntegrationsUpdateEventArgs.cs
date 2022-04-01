using System.Text.Json.Serialization;

namespace NetCord.JsonModels.EventArgs;

internal record JsonGuildIntegrationsUpdateEventArgs
{
    [JsonPropertyName("guild_id")]
    public Snowflake GuildId { get; init; }
}