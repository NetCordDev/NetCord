using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

internal partial class JsonGateway
{
    [JsonPropertyName("url")]
    public string Url { get; set; }

    [JsonSerializable(typeof(JsonGateway))]
    public partial class JsonGatewaySerializerContext : JsonSerializerContext
    {
        public static JsonGatewaySerializerContext WithOptions { get; } = new(new(ToObjectExtensions._options));
    }
}
