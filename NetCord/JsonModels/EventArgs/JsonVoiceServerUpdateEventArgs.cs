using System.Text.Json.Serialization;

namespace NetCord.JsonModels.EventArgs;

public partial class JsonVoiceServerUpdateEventArgs
{
    [JsonPropertyName("token")]
    public string Token { get; set; }

    [JsonPropertyName("guild_id")]
    public ulong GuildId { get; set; }

    [JsonPropertyName("endpoint")]
    public string? Endpoint { get; set; }

    [JsonSerializable(typeof(JsonVoiceServerUpdateEventArgs))]
    public partial class JsonVoiceServerUpdateEventArgsSerializerContext : JsonSerializerContext
    {
        public static JsonVoiceServerUpdateEventArgsSerializerContext WithOptions { get; } = new(new(ToObjectExtensions._options));
    }
}
