using System.Text.Json.Serialization;

namespace NetCord.Rest;

public class LinkButtonProperties : ButtonProperties
{
    [JsonPropertyName("url")]
    public string Url { get; }

    public LinkButtonProperties(string url) : base((ButtonStyle)5)
    {
        Url = url;
    }
}