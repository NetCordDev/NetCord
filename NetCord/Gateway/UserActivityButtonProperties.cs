using System.Text.Json.Serialization;

namespace NetCord.Gateway;

public class UserActivityButtonProperties
{
    public UserActivityButtonProperties(string label, string url)
    {
        Label = label;
        Url = url;
    }

    [JsonPropertyName("label")]
    public string Label { get; }

    [JsonPropertyName("url")]
    public string Url { get; }
}