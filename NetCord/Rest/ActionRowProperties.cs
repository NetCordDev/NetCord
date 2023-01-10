using System.Text.Json.Serialization;

namespace NetCord.Rest;

public partial class ActionRowProperties : ComponentProperties
{
    /// <summary>
    /// Buttons of the action row (max 5).
    /// </summary>
    [JsonPropertyName("components")]
    public IEnumerable<ButtonProperties> Buttons { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="buttons">Buttons of the action row (max 5).</param>
    public ActionRowProperties(IEnumerable<ButtonProperties> buttons) : base(ComponentType.ActionRow)
    {
        Buttons = buttons;
    }

    [JsonSerializable(typeof(ActionRowProperties))]
    public partial class ActionRowPropertiesSerializerContext : JsonSerializerContext
    {
        public static ActionRowPropertiesSerializerContext WithOptions { get; } = new(Serialization.Options);
    }
}
