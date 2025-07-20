using System.Text.Json.Serialization;

using NetCord.JsonConverters;

namespace NetCord.Rest;

[JsonConverter(typeof(JsonSerializableConverter<IButtonProperties>))]
public partial interface IButtonProperties : IComponentSectionAccessoryProperties, IJsonSerializable
{
    /// <summary>
    /// Style of the button.
    /// </summary>
    public ButtonStyle Style { get; }

    /// <summary>
    /// Whether the button is disabled.
    /// </summary>
    public bool Disabled { get; set; }
}
