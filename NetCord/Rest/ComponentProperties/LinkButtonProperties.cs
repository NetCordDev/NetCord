using System.Text.Json.Serialization;

namespace NetCord.Rest;

public partial class LinkButtonProperties : ICustomizableButtonProperties
{
    /// <summary>
    /// Url of the button.
    /// </summary>
    [JsonPropertyName("url")]
    public string Url { get; set; }

    [JsonPropertyName("style")]
    public ButtonStyle Style => (ButtonStyle)5;

    [JsonPropertyName("type")]
    public ComponentType ComponentType => ComponentType.Button;

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("label")]
    public string? Label { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("emoji")]
    public EmojiProperties? Emoji { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("disabled")]
    public bool Disabled { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="url">Url of the button.</param>
    /// <param name="label">Text that appears on the button (max 80 characters).</param>
    public LinkButtonProperties(string url, string label)
    {
        Url = url;
        Label = label;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="url">Url of the button.</param>
    /// <param name="emoji">Emoji that appears on the button.</param>
    public LinkButtonProperties(string url, EmojiProperties emoji)
    {
        Url = url;
        Emoji = emoji;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="url">Url of the button.</param>
    /// <param name="label">Text that appears on the button (max 80 characters).</param>
    /// <param name="emoji">Emoji that appears on the button.</param>
    public LinkButtonProperties(string url, string label, EmojiProperties emoji)
    {
        Url = url;
        Label = label;
        Emoji = emoji;
    }
}
