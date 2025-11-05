using System.Text.Json;
using System.Text.Json.Serialization;

namespace NetCord.Rest;

/// <summary>
/// 
/// </summary>
/// <param name="customId"><inheritdoc cref="CustomId" path="/summary" /></param>
[GenerateMethodsForProperties]
public abstract partial class MenuProperties(string customId) : IInteractiveComponentProperties, IMessageComponentProperties, IComponentContainerComponentProperties, ILabelComponentProperties
{
    [JsonPropertyName("type")]
    public abstract ComponentType ComponentType { get; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("id")]
    public int? Id { get; set; }

    [JsonPropertyName("custom_id")]
    public string CustomId { get; set; } = customId;

    /// <summary>
    /// Placeholder text if nothing is selected (max 150 characters).
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("placeholder")]
    public string? Placeholder { get; set; }

    /// <summary>
    /// Minimum number of items that must be chosen, default 1 (0-25).
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("min_values")]
    public int? MinValues { get; set; }

    /// <summary>
    /// Maximum number of items that can be chosen, default 1 (max 25).
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("max_values")]
    public int? MaxValues { get; set; }

    /// <summary>
    /// Whether the menu is disabled.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("disabled")]
    public bool Disabled { get; set; }

    /// <summary>
    /// Whether the menu is required to answer in a modal. Defaults to <see langword="true"/>.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("required")]
    public bool? Required { get; set; }

    [JsonIgnore]
    public int? ParentId { get; set; }

    void IJsonSerializable<IMessageComponentProperties>.WriteTo(Utf8JsonWriter writer)
    {
        WriteToMessage(writer);
    }

    void IJsonSerializable<IComponentContainerComponentProperties>.WriteTo(Utf8JsonWriter writer)
    {
        WriteToMessage(writer);
    }

    void IJsonSerializable<ILabelComponentProperties>.WriteTo(Utf8JsonWriter writer)
    {
        WriteToLabel(writer);
    }

    private protected abstract void WriteToMessage(Utf8JsonWriter writer);

    private protected abstract void WriteToLabel(Utf8JsonWriter writer);
}

[GenerateMethodsForProperties]
public partial class FileUploadProperties(string customId) : IInteractiveComponentProperties, ILabelComponentProperties
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("id")]
    public int? Id { get; set; }

    [JsonPropertyName("type")]
    public ComponentType ComponentType => ComponentType.FileUpload;

    [JsonPropertyName("custom_id")]
    public string CustomId { get; set; } = customId;

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("min_values")]
    public int? MinValues { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("max_values")]
    public int? MaxValues { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("required")]
    public bool? Required { get; set; }

    void IJsonSerializable<ILabelComponentProperties>.WriteTo(Utf8JsonWriter writer)
    {
        JsonSerializer.Serialize(writer, this, Serialization.Default.FileUploadProperties);
    }
}
