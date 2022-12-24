using System.Text.Json.Serialization;

namespace NetCord.JsonModels.EventArgs;

public partial class JsonGuildRoleDeleteEventArgs
{
    [JsonPropertyName("guild_id")]
    public ulong GuildId { get; set; }

    [JsonPropertyName("role_id")]
    public ulong RoleId { get; set; }

    [JsonSerializable(typeof(JsonGuildRoleDeleteEventArgs))]
    public partial class JsonGuildRoleDeleteEventArgsSerializerContext : JsonSerializerContext
    {
        public static JsonGuildRoleDeleteEventArgsSerializerContext WithOptions { get; } = new(Serialization.Options);
    }
}
