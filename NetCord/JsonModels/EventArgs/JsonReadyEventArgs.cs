using System.Text.Json.Serialization;

using NetCord.Gateway;

namespace NetCord.JsonModels.EventArgs;

public partial class JsonReadyEventArgs
{
    [JsonPropertyName("v")]
    public GatewayVersion Version { get; set; }

    [JsonPropertyName("user")]
    public JsonUser User { get; set; }

    [JsonPropertyName("guilds")]
    public JsonEntity[] Guilds { get; set; }

    [JsonPropertyName("session_id")]
    public string SessionId { get; set; }

    [JsonPropertyName("shard")]
    public Shard? Shard { get; set; }

    [JsonPropertyName("application")]
    public JsonApplication Application { get; set; }

    [JsonPropertyName("private_channels")]
    public JsonChannel[] DMChannels { get; set; }

    [JsonSerializable(typeof(JsonReadyEventArgs))]
    public partial class JsonReadyEventArgsSerializerContext : JsonSerializerContext
    {
        public static JsonReadyEventArgsSerializerContext WithOptions { get; } = new(new(ToObjectExtensions._options));
    }
}
