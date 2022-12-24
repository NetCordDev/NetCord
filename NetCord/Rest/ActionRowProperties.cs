using System.Text.Json.Serialization;

namespace NetCord.Rest;

public partial class ActionRowProperties : ComponentProperties
{
    [JsonPropertyName("components")]
    public IEnumerable<ButtonProperties> Buttons { get; }

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
