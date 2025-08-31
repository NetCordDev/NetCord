using System.Collections;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace NetCord.Rest;

#pragma warning disable IDE0028 // Simplify collection initialization
#pragma warning disable IDE0306 // Simplify collection initialization

/// <summary>
/// 
/// </summary>
/// <param name="components">Components of the action row (max 5).</param>
[CollectionBuilder(typeof(ActionRowProperties), nameof(Create))]
[GenerateMethodsForProperties]
public partial class ActionRowProperties(IEnumerable<IActionRowComponentProperties> components) : IMessageComponentProperties, IComponentContainerComponentProperties, IEnumerable<IActionRowComponentProperties>
{
    private static readonly JsonEncodedText _type = JsonEncodedText.Encode("type");
    private static readonly JsonEncodedText _components = JsonEncodedText.Encode("components");
    private static readonly JsonEncodedText _id = JsonEncodedText.Encode("id");

    public ActionRowProperties() : this([])
    {
    }

    [JsonPropertyName("type")]
    public ComponentType ComponentType => ComponentType.ActionRow;

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("id")]
    public int? Id { get; set; }

    /// <summary>
    /// Components of the action row (max 5).
    /// </summary>
    [JsonPropertyName("components")]
    public IEnumerable<IActionRowComponentProperties> Components { get; set; } = components;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public void Add(IActionRowComponentProperties component) => AddComponents(component);

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static ActionRowProperties Create(ReadOnlySpan<IActionRowComponentProperties> components) => new(components.ToArray());

    IEnumerator<IActionRowComponentProperties> IEnumerable<IActionRowComponentProperties>.GetEnumerator() => Components.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)Components).GetEnumerator();

    private static void WriteActionRowStart(Utf8JsonWriter writer, int? actionRowId)
    {
        writer.WriteStartObject();

        writer.WriteNumber(_type, 1);

        if (actionRowId.HasValue)
            writer.WriteNumber(_id, actionRowId.GetValueOrDefault());

        writer.WritePropertyName(_components);
    }

    internal static void WriteActionRowLike<T>(Utf8JsonWriter writer, int? actionRowId, T value, JsonTypeInfo<T> jsonTypeInfo)
    {
        WriteActionRowStart(writer, actionRowId);

        writer.WriteStartArray();

        JsonSerializer.Serialize(writer, value, jsonTypeInfo);

        writer.WriteEndArray();

        writer.WriteEndObject();
    }

    private void WriteTo(Utf8JsonWriter writer)
    {
        WriteActionRowStart(writer, Id);

        JsonSerializer.Serialize(writer, Components, Serialization.Default.IEnumerableIActionRowComponentProperties);

        writer.WriteEndObject();
    }

    void IJsonSerializable<IMessageComponentProperties>.WriteTo(Utf8JsonWriter writer)
    {
        WriteTo(writer);
    }

    void IJsonSerializable<IComponentContainerComponentProperties>.WriteTo(Utf8JsonWriter writer)
    {
        WriteTo(writer);
    }
}
