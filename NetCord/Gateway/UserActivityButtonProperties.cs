using System.Text.Json.Serialization;

namespace NetCord.Gateway;

public partial class UserActivityButtonProperties
{
    public UserActivityButtonProperties(string label, string url)
    {
        Label = label;
        Url = url;
    }

    [JsonPropertyName("label")]
    public string Label { get; set; }

    [JsonPropertyName("url")]
    public string Url { get; set; }
}
