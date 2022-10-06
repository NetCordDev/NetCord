using System.Text.Json.Serialization;

namespace NetCord.JsonModels.EventArgs;

public partial class JsonGuildRoleDeleteEventArgs
{
    [JsonPropertyName("guild_id")]
    public Snowflake GuildId { get; set; }

    [JsonPropertyName("role_id")]
    public Snowflake RoleId { get; set; }

    [JsonSerializable(typeof(JsonGuildRoleDeleteEventArgs))]
    public partial class JsonGuildRoleDeleteEventArgsSerializerContext : JsonSerializerContext
    {
        public static JsonGuildRoleDeleteEventArgsSerializerContext WithOptions { get; } = new(new(ToObjectExtensions._options));
    }
}
