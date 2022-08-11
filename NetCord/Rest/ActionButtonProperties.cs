using System.Text.Json.Serialization;

namespace NetCord.Rest;

[JsonSourceGenerationOptions(DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
public class ActionButtonProperties : ButtonProperties
{
    [JsonPropertyName("custom_id")]
    public string CustomId { get; }

    public ActionButtonProperties(string customId, string label, ButtonStyle style) : base(style, label)
    {
        CustomId = customId;
    }

    public ActionButtonProperties(string customId, EmojiProperties emoji, ButtonStyle style) : base(style, emoji)
    {
        CustomId = customId;
    }

    public ActionButtonProperties(string customId, string label, EmojiProperties emoji, ButtonStyle style) : base(style, label, emoji)
    {
        CustomId = customId;
    }
}