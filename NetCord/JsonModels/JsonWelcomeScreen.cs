using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

internal record JsonWelcomeScreen
{
    [JsonPropertyName("description")]
    public string Description { get; init; }

    [JsonPropertyName("welcome_channels")]
    public JsonChannel[] WelcomeChannels { get; init; }
}
