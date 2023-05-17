using System.Text.Json.Serialization;

using NetCord.JsonModels;

namespace NetCord.Gateway.JsonModels.EventArgs;

public partial class JsonGuildUserRemoveEventArgs
{
    [JsonPropertyName("guild_id")]
    public ulong GuildId { get; set; }

    [JsonPropertyName("user")]
    public JsonUser User { get; set; }

    [JsonSerializable(typeof(JsonGuildUserRemoveEventArgs))]
    public partial class JsonGuildUserRemoveEventArgsSerializerContext : JsonSerializerContext
    {
        public static JsonGuildUserRemoveEventArgsSerializerContext WithOptions { get; } = new(Serialization.Options);
    }
}
