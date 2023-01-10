using System.Text.Json.Serialization;

namespace NetCord.Rest;

public partial class LinkButtonProperties : ButtonProperties
{
    /// <summary>
    /// Url of the button.
    /// </summary>
    [JsonPropertyName("url")]
    public string Url { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="url">Url of the button.</param>
    /// <param name="label">Text that appears on the button (max 80 characters).</param>
    public LinkButtonProperties(string url, string label) : base(label, (ButtonStyle)5)
    {
        Url = url;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="url">Url of the button.</param>
    /// <param name="emoji">Emoji that appears on the button.</param>
    public LinkButtonProperties(string url, EmojiProperties emoji) : base(emoji, (ButtonStyle)5)
    {
        Url = url;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="url">Url of the button.</param>
    /// <param name="label">Text that appears on the button (max 80 characters).</param>
    /// <param name="emoji">Emoji that appears on the button.</param>
    public LinkButtonProperties(string url, string label, EmojiProperties emoji) : base(label, emoji, (ButtonStyle)5)
    {
        Url = url;
    }

    [JsonSerializable(typeof(LinkButtonProperties))]
    public partial class LinkButtonPropertiesSerializerContext : JsonSerializerContext
    {
        public static LinkButtonPropertiesSerializerContext WithOptions { get; } = new(Serialization.Options);
    }
}
