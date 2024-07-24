using System.Text.Json;
using System.Text.Json.Serialization;

namespace NetCord.Rest;

[JsonConverter(typeof(IButtonConverter))]
public partial interface IButtonProperties
{
    /// <summary>
    /// Style of the button.
    /// </summary>
    public ButtonStyle Style { get; }

    /// <summary>
    /// Type of the component.
    /// </summary>
    public ComponentType ComponentType { get; }

    /// <summary>
    /// Whether the button is disabled.
    /// </summary>
    public bool Disabled { get; set; }

    public class IButtonConverter : JsonConverter<IButtonProperties>
    {
        public override IButtonProperties Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => throw new NotImplementedException();

        public override void Write(Utf8JsonWriter writer, IButtonProperties button, JsonSerializerOptions options)
        {
            switch (button)
            {
                case ButtonProperties buttonProperties:
                    JsonSerializer.Serialize(writer, buttonProperties, Serialization.Default.ButtonProperties);
                    break;
                case LinkButtonProperties linkButtonProperties:
                    JsonSerializer.Serialize(writer, linkButtonProperties, Serialization.Default.LinkButtonProperties);
                    break;
                case PremiumButtonProperties premiumButtonProperties:
                    JsonSerializer.Serialize(writer, premiumButtonProperties, Serialization.Default.PremiumButtonProperties);
                    break;
                default:
                    throw new InvalidOperationException($"Invalid {nameof(IButtonProperties)} value.");
            }
        }
    }
}
