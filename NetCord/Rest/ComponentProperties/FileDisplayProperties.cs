using System.Text.Json;
using System.Text.Json.Serialization;

namespace NetCord.Rest;

/// <summary>
/// Represents a file component to be sent in a message.
/// </summary>
/// <param name="file">The file to be sent as a component. The file must be attached to the message for it to be displayed correctly, and the URL must use the attachment:// protocol.</param>
[GenerateMethodsForProperties]
public partial class FileDisplayProperties(ComponentMediaProperties file) : IMessageComponentProperties, IComponentContainerComponentProperties
{
    [JsonPropertyName("type")]
    public ComponentType ComponentType => ComponentType.File;

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("id")]
    public int? Id { get; set; }

    /// <summary>
    /// The file to be sent as a component.
    /// </summary>
    /// <remarks>
    /// The file must be attached to the message for it to be displayed correctly.
    /// The URL must use the attachment:// protocol.
    /// </remarks>
    [JsonPropertyName("file")]
    public ComponentMediaProperties File { get; set; } = file;

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("spoiler")]
    public bool Spoiler { get; set; }

    private void WriteTo(Utf8JsonWriter writer)
    {
        JsonSerializer.Serialize(writer, this, Serialization.Default.FileDisplayProperties);
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
