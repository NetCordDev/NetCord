using System.Text.Json.Serialization;

using NetCord.JsonConverters;

namespace NetCord.Rest;

[JsonConverter(typeof(JsonSerializableConverter<IActionRowComponentProperties>))]
public partial interface IActionRowComponentProperties : IComponentProperties, IJsonSerializable<IActionRowComponentProperties>;

[JsonConverter(typeof(JsonSerializableConverter<IComponentSectionAccessoryComponentProperties>))]
public partial interface IComponentSectionAccessoryComponentProperties : IComponentProperties, IJsonSerializable<IComponentSectionAccessoryComponentProperties>;

[JsonConverter(typeof(JsonSerializableConverter<IComponentSectionComponentProperties>))]
public partial interface IComponentSectionComponentProperties : IComponentProperties, IJsonSerializable<IComponentSectionComponentProperties>;

[JsonConverter(typeof(JsonSerializableConverter<IComponentContainerComponentProperties>))]
public partial interface IComponentContainerComponentProperties : IComponentProperties, IJsonSerializable<IComponentContainerComponentProperties>;

[JsonConverter(typeof(JsonSerializableConverter<IMessageComponentProperties>))]
public partial interface IMessageComponentProperties : IComponentProperties, IJsonSerializable<IMessageComponentProperties>;

[JsonConverter(typeof(JsonSerializableConverter<IModalComponentProperties>))]
public partial interface IModalComponentProperties : IComponentProperties, IJsonSerializable<IModalComponentProperties>;

[JsonConverter(typeof(JsonSerializableConverter<ILabelComponentProperties>))]
public partial interface ILabelComponentProperties : IComponentProperties, IJsonSerializable<ILabelComponentProperties>;

public partial interface IInteractiveComponentProperties : IComponentProperties
{
    /// <summary>
    /// Developer-defined identifier for the button (max 100 characters).
    /// </summary>
    [JsonPropertyName("custom_id")]
    public string CustomId { get; set; }
}

public partial interface IComponentProperties
{
    /// <summary>
    /// Type of the component.
    /// </summary>
    [JsonPropertyName("type")]
    public ComponentType ComponentType { get; }

    /// <summary>
    /// Unique identifier for the component. Auto populated through increment if not provided.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("id")]
    public int? Id { get; set; }
}
