using System.Text.Json.Serialization;

namespace NetCord.Rest.JsonModels;

internal partial class JsonGateway
{
    [JsonPropertyName("url")]
    public string Url { get; set; }

    [JsonSerializable(typeof(JsonGateway))]
    public partial class JsonGatewaySerializerContext : JsonSerializerContext
    {
        public static JsonGatewaySerializerContext WithOptions { get; } = new(Serialization.Options);
    }
}
