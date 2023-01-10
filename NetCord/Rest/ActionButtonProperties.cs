using System.Text.Json.Serialization;

namespace NetCord.Rest;

[JsonSourceGenerationOptions(DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
public partial class ActionButtonProperties : ButtonProperties
{
    /// <summary>
    /// Developer-defined identifier for the button (max 100 characters).
    /// </summary>
    [JsonPropertyName("custom_id")]
    public string CustomId { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="customId">Developer-defined identifier for the button (max 100 characters).</param>
    /// <param name="label">Text that appears on the button (max 80 characters).</param>
    /// <param name="style">Style of the button.</param>
    public ActionButtonProperties(string customId, string label, ButtonStyle style) : base(label, style)
    {
        CustomId = customId;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="customId">Developer-defined identifier for the button (max 100 characters).</param>
    /// <param name="emoji">Emoji that appears on the button.</param>
    /// <param name="style">Style of the button.</param>
    public ActionButtonProperties(string customId, EmojiProperties emoji, ButtonStyle style) : base(emoji, style)
    {
        CustomId = customId;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="customId">Developer-defined identifier for the button (max 100 characters).</param>
    /// <param name="label">Text that appears on the button (max 80 characters).</param>
    /// <param name="emoji">Emoji that appears on the button.</param>
    /// <param name="style">Style of the button.</param>
    public ActionButtonProperties(string customId, string label, EmojiProperties emoji, ButtonStyle style) : base(label, emoji, style)
    {
        CustomId = customId;
    }

    [JsonSerializable(typeof(ActionButtonProperties))]
    public partial class ActionButtonPropertiesSerializerContext : JsonSerializerContext
    {
        public static ActionButtonPropertiesSerializerContext WithOptions { get; } = new(Serialization.Options);
    }
}
