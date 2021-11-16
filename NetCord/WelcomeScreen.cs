using System.Text.Json.Serialization;

namespace NetCord
{
    public class WelcomeScreen
    {
        [JsonInclude]
        [JsonPropertyName("description")]
        public string Description { get; private init; }
        [JsonInclude]
        [JsonPropertyName("welcome_channels")]
        public IEnumerable<Channel> WelcomeChannels { get; private init; }
    }
}