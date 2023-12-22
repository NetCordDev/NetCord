using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public class JsonGuildWelcomeScreen
{
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("welcome_channels")]
    public JsonWelcomeScreenChannel[] WelcomeChannels { get; set; }
}
