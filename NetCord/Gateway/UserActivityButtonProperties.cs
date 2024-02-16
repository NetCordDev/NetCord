using System.Text.Json.Serialization;

namespace NetCord.Gateway;

public partial class UserActivityButtonProperties(string label, string url)
{
    [JsonPropertyName("label")]
    public string Label { get; set; } = label;

    [JsonPropertyName("url")]
    public string Url { get; set; } = url;
}
