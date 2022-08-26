using System.Text.Json.Serialization;

namespace NetCord.Rest;

public class GuildWelcomeScreenOptions
{
    internal GuildWelcomeScreenOptions()
    {
    }

    [JsonPropertyName("enabled")]
    public bool? Enabled { get; set; }

    [JsonPropertyName("welcome_channels")]
    public IEnumerable<GuildWelcomeScreenChannelProperties>? WelcomeChannels { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }
}
