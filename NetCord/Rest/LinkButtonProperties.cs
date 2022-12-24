using System.Text.Json.Serialization;

namespace NetCord.Rest;

public partial class LinkButtonProperties : ButtonProperties
{
    [JsonPropertyName("url")]
    public string Url { get; }

    public LinkButtonProperties(string url, string label) : base((ButtonStyle)5, label)
    {
        Url = url;
    }

    public LinkButtonProperties(string url, EmojiProperties emoji) : base((ButtonStyle)5, emoji)
    {
        Url = url;
    }

    public LinkButtonProperties(string url, string label, EmojiProperties emoji) : base((ButtonStyle)5, label, emoji)
    {
        Url = url;
    }

    [JsonSerializable(typeof(LinkButtonProperties))]
    public partial class LinkButtonPropertiesSerializerContext : JsonSerializerContext
    {
        public static LinkButtonPropertiesSerializerContext WithOptions { get; } = new(Serialization.Options);
    }
}
