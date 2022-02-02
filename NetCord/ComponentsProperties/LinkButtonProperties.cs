using System.Text.Json.Serialization;

namespace NetCord
{
    [JsonSourceGenerationOptions(DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
    public class LinkButtonProperties : ButtonProperties
    {
        private const ButtonStyle style = (ButtonStyle)5;

        [JsonPropertyName("url")]
        public Uri Uri { get; }

        public LinkButtonProperties(string label, Uri uri) : base(label, style)
        {
            if (!uri.IsAbsoluteUri)
                throw new UriFormatException($"Invalid {nameof(uri)}");
            if (uri.Scheme is not "https" and not "http" and not "discord")
                throw new UriFormatException($"Invalid {nameof(uri)} scheme");
            Uri = uri;
        }
    }
}
