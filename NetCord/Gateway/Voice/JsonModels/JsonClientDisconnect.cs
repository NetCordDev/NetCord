using System.Text.Json.Serialization;

namespace NetCord.Gateway.Voice.JsonModels;

internal partial class JsonClientDisconnect
{
    [JsonPropertyName("user_id")]
    public ulong UserId { get; set; }

    [JsonSerializable(typeof(JsonClientDisconnect))]
    public partial class JsonClientDisconnectSerializerContext : JsonSerializerContext
    {
        public static JsonClientDisconnectSerializerContext WithOptions { get; } = new(Serialization.Options);
    }
}
