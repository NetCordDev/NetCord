using System.Text.Json.Serialization;

namespace NetCord.Rest;

public abstract partial class ComponentProperties
{
    /// <summary>
    /// Type of the component.
    /// </summary>
    [JsonPropertyName("type")]
    public abstract ComponentType ComponentType { get; }
}
