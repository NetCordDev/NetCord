using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public record JsonApplicationCommandGuildPermissions
{
    [JsonPropertyName("id")]
    public Snowflake CommandId { get; init; }

    [JsonPropertyName("application_id")]
    public Snowflake ApplicationId { get; init; }

    [JsonPropertyName("guild_id")]
    public Snowflake GuildId { get; init; }

    [JsonPropertyName("permissions")]
    public JsonApplicationCommandGuildPermission[] Permissions { get; init; }
}
