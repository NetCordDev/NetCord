using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public partial class JsonApplicationCommandGuildPermissions
{
    [JsonPropertyName("id")]
    public Snowflake CommandId { get; set; }

    [JsonPropertyName("application_id")]
    public Snowflake ApplicationId { get; set; }

    [JsonPropertyName("guild_id")]
    public Snowflake GuildId { get; set; }

    [JsonPropertyName("permissions")]
    public JsonApplicationCommandGuildPermission[] Permissions { get; set; }

    [JsonSerializable(typeof(JsonApplicationCommandGuildPermissions))]
    public partial class JsonApplicationCommandGuildPermissionsSerializerContext : JsonSerializerContext
    {
        public static JsonApplicationCommandGuildPermissionsSerializerContext WithOptions { get; } = new(new(ToObjectExtensions._options));
    }

    [JsonSerializable(typeof(JsonApplicationCommandGuildPermissions[]))]
    public partial class JsonApplicationCommandGuildPermissionsArraySerializerContext : JsonSerializerContext
    {
        public static JsonApplicationCommandGuildPermissionsArraySerializerContext WithOptions { get; } = new(new(ToObjectExtensions._options));
    }
}
