using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public partial class JsonWelcomeScreen
{
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("welcome_channels")]
    public JsonWelcomeScreenChannel[] WelcomeChannels { get; set; }

    [JsonSerializable(typeof(JsonWelcomeScreen))]
    public partial class JsonWelcomeScreenSerializerContext : JsonSerializerContext
    {
        public static JsonWelcomeScreenSerializerContext WithOptions { get; } = new(new(ToObjectExtensions._options));
    }
}
