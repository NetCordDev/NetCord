using System.Text.Json.Serialization;

using NetCord.JsonModels;

namespace NetCord.Rest.JsonModels;

public class JsonApplicationCommandGuildPermissions
{
    [JsonPropertyName("id")]
    public ulong CommandId { get; set; }

    [JsonPropertyName("application_id")]
    public ulong ApplicationId { get; set; }

    [JsonPropertyName("guild_id")]
    public ulong GuildId { get; set; }

    [JsonPropertyName("permissions")]
    public JsonApplicationCommandGuildPermission[] Permissions { get; set; }
}
