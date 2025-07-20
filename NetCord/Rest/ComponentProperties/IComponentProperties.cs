using System.Text.Json.Serialization;

using NetCord.JsonConverters;

namespace NetCord.Rest;

[JsonConverter(typeof(JsonSerializableConverter<IComponentProperties>))]
public partial interface IComponentProperties : IJsonSerializable
{
    /// <summary>
    /// Unique identifier for the component. Auto populated through increment if not provided.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("id")]
    public int? Id { get; set; }

    /// <summary>
    /// Type of the component.
    /// </summary>
    [JsonPropertyName("type")]
    public ComponentType ComponentType { get; }
}
