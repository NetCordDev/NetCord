using System.Text.Json.Serialization;

using NetCord.JsonConverters;

namespace NetCord.Rest;

[JsonConverter(typeof(JsonSerializableConverter<IActionRowComponentProperties>))]
[GenerateMethodsForProperties]
public partial interface IActionRowComponentProperties : IComponentProperties, IJsonSerializable<IActionRowComponentProperties>;

[JsonConverter(typeof(JsonSerializableConverter<IComponentSectionAccessoryComponentProperties>))]
[GenerateMethodsForProperties]
public partial interface IComponentSectionAccessoryComponentProperties : IComponentProperties, IJsonSerializable<IComponentSectionAccessoryComponentProperties>;

[JsonConverter(typeof(JsonSerializableConverter<IComponentSectionComponentProperties>))]
[GenerateMethodsForProperties]
public partial interface IComponentSectionComponentProperties : IComponentProperties, IJsonSerializable<IComponentSectionComponentProperties>;

[JsonConverter(typeof(JsonSerializableConverter<IComponentContainerComponentProperties>))]
[GenerateMethodsForProperties]
public partial interface IComponentContainerComponentProperties : IComponentProperties, IJsonSerializable<IComponentContainerComponentProperties>;

[JsonConverter(typeof(JsonSerializableConverter<IMessageComponentProperties>))]
[GenerateMethodsForProperties]
public partial interface IMessageComponentProperties : IComponentProperties, IJsonSerializable<IMessageComponentProperties>;

[JsonConverter(typeof(JsonSerializableConverter<IModalComponentProperties>))]
[GenerateMethodsForProperties]
public partial interface IModalComponentProperties : IComponentProperties, IJsonSerializable<IModalComponentProperties>;

[JsonConverter(typeof(JsonSerializableConverter<ILabelComponentProperties>))]
[GenerateMethodsForProperties]
public partial interface ILabelComponentProperties : IComponentProperties, IJsonSerializable<ILabelComponentProperties>;

[GenerateMethodsForProperties]
public partial interface IInteractiveComponentProperties : IComponentProperties
{
    /// <summary>
    /// Developer-defined identifier for the button (max 100 characters).
    /// </summary>
    [JsonPropertyName("custom_id")]
    public string CustomId { get; set; }
}

[GenerateMethodsForProperties]
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
