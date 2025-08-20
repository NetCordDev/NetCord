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
/// <param name="buttons">Buttons of the action row (max 5).</param>
[CollectionBuilder(typeof(ActionRowProperties), nameof(Create))]
[GenerateMethodsForProperties]
public partial class ActionRowProperties(IEnumerable<IButtonProperties> buttons) : IComponentProperties, IEnumerable<IButtonProperties>
{
    private static readonly JsonEncodedText _type = JsonEncodedText.Encode("type");
    private static readonly JsonEncodedText _components = JsonEncodedText.Encode("components");
    private static readonly JsonEncodedText _id = JsonEncodedText.Encode("id");

    public ActionRowProperties() : this([])
    {
    }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("id")]
    public int? Id { get; set; }

    [JsonPropertyName("type")]
    public ComponentType ComponentType => ComponentType.ActionRow;

    /// <summary>
    /// Buttons of the action row (max 5).
    /// </summary>
    [JsonPropertyName("components")]
    public IEnumerable<IButtonProperties> Buttons { get; set; } = buttons;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public void Add(IButtonProperties button) => AddButtons(button);

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static ActionRowProperties Create(ReadOnlySpan<IButtonProperties> buttons) => new(buttons.ToArray());

    IEnumerator<IButtonProperties> IEnumerable<IButtonProperties>.GetEnumerator() => Buttons.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)Buttons).GetEnumerator();

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

    public void WriteTo(Utf8JsonWriter writer)
    {
        WriteActionRowStart(writer, Id);

        JsonSerializer.Serialize(writer, Buttons, Serialization.Default.IEnumerableIButtonProperties);

        writer.WriteEndObject();
    }
}
