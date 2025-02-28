using System.Text.Json.Serialization;

namespace NetCord.Rest;

public partial class ComponentMediaProperties(string url)
{
    [JsonPropertyName("url")]
    public string Url { get; set; } = url;
}
