using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public partial class JsonGuildWelcomeScreen
{
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("welcome_channels")]
    public JsonWelcomeScreenChannel[] WelcomeChannels { get; set; }

    [JsonSerializable(typeof(JsonGuildWelcomeScreen))]
    public partial class JsonGuildWelcomeScreenSerializerContext : JsonSerializerContext
    {
        public static JsonGuildWelcomeScreenSerializerContext WithOptions { get; } = new(Serialization.Options);
    }
}
