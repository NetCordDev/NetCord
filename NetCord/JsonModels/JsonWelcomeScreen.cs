using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public record JsonWelcomeScreen
{
    [JsonPropertyName("description")]
    public string? Description { get; init; }

    [JsonPropertyName("welcome_channels")]
    public JsonWelcomeScreenChannel[] WelcomeChannels { get; init; }
}
