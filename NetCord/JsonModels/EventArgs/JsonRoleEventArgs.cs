using System.Text.Json.Serialization;

namespace NetCord.JsonModels.EventArgs;

public partial class JsonRoleEventArgs
{
    [JsonPropertyName("guild_id")]
    public ulong GuildId { get; set; }

    [JsonPropertyName("role")]
    public JsonRole Role { get; set; }

    [JsonSerializable(typeof(JsonRoleEventArgs))]
    public partial class JsonRoleEventArgsSerializerContext : JsonSerializerContext
    {
        public static JsonRoleEventArgsSerializerContext WithOptions { get; } = new(Serialization.Options);
    }
}
