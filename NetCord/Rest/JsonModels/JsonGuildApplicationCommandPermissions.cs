using System.Text.Json.Serialization;

using NetCord.JsonModels;

namespace NetCord.Rest.JsonModels;

public partial class JsonApplicationCommandGuildPermissions
{
    [JsonPropertyName("id")]
    public ulong CommandId { get; set; }

    [JsonPropertyName("application_id")]
    public ulong ApplicationId { get; set; }

    [JsonPropertyName("guild_id")]
    public ulong GuildId { get; set; }

    [JsonPropertyName("permissions")]
    public JsonApplicationCommandGuildPermission[] Permissions { get; set; }

    [JsonSerializable(typeof(JsonApplicationCommandGuildPermissions))]
    public partial class JsonApplicationCommandGuildPermissionsSerializerContext : JsonSerializerContext
    {
        public static JsonApplicationCommandGuildPermissionsSerializerContext WithOptions { get; } = new(Serialization.Options);
    }

    [JsonSerializable(typeof(JsonApplicationCommandGuildPermissions[]))]
    public partial class JsonApplicationCommandGuildPermissionsArraySerializerContext : JsonSerializerContext
    {
        public static JsonApplicationCommandGuildPermissionsArraySerializerContext WithOptions { get; } = new(Serialization.Options);
    }
}
