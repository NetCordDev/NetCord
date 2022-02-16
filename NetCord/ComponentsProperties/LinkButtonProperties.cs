using System.Text.Json.Serialization;

namespace NetCord;

public class LinkButtonProperties : ButtonProperties
{
    [JsonPropertyName("url")]
    public Uri Uri { get; }

    public LinkButtonProperties(Uri uri) : base((ButtonStyle)5)
    {
        if (!uri.IsAbsoluteUri)
            throw new UriFormatException($"Invalid {nameof(uri)}");
        if (uri.Scheme is not "https" and not "http" and not "discord")
            throw new UriFormatException($"Invalid {nameof(uri)} scheme");
        Uri = uri;
    }
}