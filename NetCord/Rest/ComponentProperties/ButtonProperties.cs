using System.Text.Json.Serialization;

namespace NetCord.Rest;

public partial class ButtonProperties : IButtonProperties
{
    /// <summary>
    /// Developer-defined identifier for the button (max 100 characters).
    /// </summary>
    [JsonPropertyName("custom_id")]
    public string CustomId { get; set; }

    [JsonPropertyName("style")]
    public ButtonStyle Style { get; set; }

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
    /// <param name="customId">Developer-defined identifier for the button (max 100 characters).</param>
    /// <param name="label">Text that appears on the button (max 80 characters).</param>
    /// <param name="style">Style of the button.</param>
    public ButtonProperties(string customId, string label, ButtonStyle style)
    {
        CustomId = customId;
        Label = label;
        Style = style;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="customId">Developer-defined identifier for the button (max 100 characters).</param>
    /// <param name="emoji">Emoji that appears on the button.</param>
    /// <param name="style">Style of the button.</param>
    public ButtonProperties(string customId, EmojiProperties emoji, ButtonStyle style)
    {
        CustomId = customId;
        Emoji = emoji;
        Style = style;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="customId">Developer-defined identifier for the button (max 100 characters).</param>
    /// <param name="label">Text that appears on the button (max 80 characters).</param>
    /// <param name="emoji">Emoji that appears on the button.</param>
    /// <param name="style">Style of the button.</param>
    public ButtonProperties(string customId, string label, EmojiProperties emoji, ButtonStyle style)
    {
        CustomId = customId;
        Label = label;
        Emoji = emoji;
        Style = style;
    }
}
