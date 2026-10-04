using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public class JsonApplicationCommandGuildPermissions
{
    [JsonPropertyName("id")]
    public required ulong CommandId { get; set; }

    [JsonPropertyName("application_id")]
    public required ulong ApplicationId { get; set; }

    [JsonPropertyName("guild_id")]
    public required ulong GuildId { get; set; }

    [JsonPropertyName("permissions")]
    public required JsonApplicationCommandGuildPermission[] Permissions { get; set; }
}
