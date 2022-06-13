using System.Text.Json.Serialization;

namespace NetCord.JsonModels.EventArgs;

public record JsonGuildIntegrationsUpdateEventArgs
{
    [JsonPropertyName("guild_id")]
    public Snowflake GuildId { get; init; }
}