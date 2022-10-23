using System.Text.Json.Serialization;

namespace NetCord.JsonModels.EventArgs;

public partial class JsonGuildRoleEventArgs
{
    [JsonPropertyName("guild_id")]
    public ulong GuildId { get; set; }

    [JsonPropertyName("role")]
    public JsonGuildRole Role { get; set; }

    [JsonSerializable(typeof(JsonGuildRoleEventArgs))]
    public partial class JsonGuildRoleEventArgsSerializerContext : JsonSerializerContext
    {
        public static JsonGuildRoleEventArgsSerializerContext WithOptions { get; } = new(new(ToObjectExtensions._options));
    }
}
