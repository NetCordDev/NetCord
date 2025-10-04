using System.Text.Json;
using System.Text.Json.Serialization;

namespace NetCord.Rest;

[GenerateMethodsForProperties]
public partial class ButtonProperties : IInteractiveComponentProperties, ICustomizableButtonProperties, IComponentSectionAccessoryComponentProperties
{
    [JsonPropertyName("type")]
    public ComponentType ComponentType => ComponentType.Button;

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("id")]
    public int? Id { get; set; }

    [JsonPropertyName("custom_id")]
    public string CustomId { get; set; }

    [JsonPropertyName("style")]
    public ButtonStyle Style { get; set; }

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

    private void WriteTo(Utf8JsonWriter writer)
    {
        JsonSerializer.Serialize(writer, this, Serialization.Default.ButtonProperties);
    }

    void IJsonSerializable<IActionRowComponentProperties>.WriteTo(Utf8JsonWriter writer)
    {
        WriteTo(writer);
    }

    void IJsonSerializable<IComponentSectionAccessoryComponentProperties>.WriteTo(Utf8JsonWriter writer)
    {
        WriteTo(writer);
    }
}
