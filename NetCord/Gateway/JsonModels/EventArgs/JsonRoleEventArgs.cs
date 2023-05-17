using System.Text.Json.Serialization;

using NetCord.JsonModels;

namespace NetCord.Gateway.JsonModels.EventArgs;

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
