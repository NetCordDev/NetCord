using System.Text.Json.Serialization;

namespace NetCord.JsonModels;
public partial class JsonUserActivitySecrets
{
    [JsonPropertyName("join")]
    public string? Join { get; set; }

    [JsonPropertyName("spectate")]
    public string? Spectate { get; set; }

    [JsonPropertyName("match")]
    public string? Match { get; set; }

    [JsonSerializable(typeof(JsonUserActivitySecrets))]
    public partial class JsonUserActivitySecretsSerializerContext : JsonSerializerContext
    {
        public static JsonUserActivitySecretsSerializerContext WithOptions { get; } = new(new(ToObjectExtensions._options));
    }
}
