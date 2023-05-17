using System.Text.Json.Serialization;

namespace NetCord.Gateway.JsonModels.EventArgs;

public partial class JsonRoleDeleteEventArgs
{
    [JsonPropertyName("guild_id")]
    public ulong GuildId { get; set; }

    [JsonPropertyName("role_id")]
    public ulong RoleId { get; set; }

    [JsonSerializable(typeof(JsonRoleDeleteEventArgs))]
    public partial class JsonRoleDeleteEventArgsSerializerContext : JsonSerializerContext
    {
        public static JsonRoleDeleteEventArgsSerializerContext WithOptions { get; } = new(Serialization.Options);
    }
}
