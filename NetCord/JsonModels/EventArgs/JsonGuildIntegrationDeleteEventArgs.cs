using System.Text.Json.Serialization;

namespace NetCord.JsonModels.EventArgs;

public record JsonGuildIntegrationDeleteEventArgs
{
    [JsonPropertyName("id")]
    public Snowflake IntegrationId { get; init; }

    [JsonPropertyName("guild_id")]
    public Snowflake GuildId { get; init; }

    [JsonPropertyName("application_id")]
    public Snowflake? ApplicationId { get; init; }
}