using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;

using NetCord.JsonConverters;

using JsonGuard;

namespace NetCord.JsonModels;

[JsonConverter(typeof(JsonComponentConverter))]
public abstract class JsonComponent
{
    [JsonPropertyName("type")]
    public ComponentType Type { get; set; }

    [JsonPropertyName("id")]
    public int Id { get; set; }

    public class JsonComponentConverter : JsonConverter<JsonComponent>
    {
        internal class JsonUnknownComponent : JsonComponent;

        public override JsonComponent? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var readerCopy = reader;

            if (!JsonConverterHelper.TrySkipToProperty(ref readerCopy, "type"u8))
                ThrowMissingTypeProperty();

            return (ComponentType)readerCopy.GetInt32() switch
            {
                ComponentType.ActionRow => JsonSerializer.Deserialize(ref reader, Serialization.Default.JsonActionRowComponent),
                ComponentType.Button => JsonSerializer.Deserialize(ref reader, Serialization.Default.JsonButtonComponent),
                ComponentType.StringSelect => JsonSerializer.Deserialize(ref reader, Serialization.Default.JsonStringSelectComponent),
                ComponentType.TextInput => JsonSerializer.Deserialize(ref reader, Serialization.Default.JsonTextInputComponent),
                ComponentType.UserSelect => JsonSerializer.Deserialize(ref reader, Serialization.Default.JsonUserSelectComponent),
                ComponentType.RoleSelect => JsonSerializer.Deserialize(ref reader, Serialization.Default.JsonRoleSelectComponent),
                ComponentType.MentionableSelect => JsonSerializer.Deserialize(ref reader, Serialization.Default.JsonMentionableSelectComponent),
                ComponentType.ChannelSelect => JsonSerializer.Deserialize(ref reader, Serialization.Default.JsonChannelSelectComponent),
                ComponentType.Section => JsonSerializer.Deserialize(ref reader, Serialization.Default.JsonComponentSectionComponent),
                ComponentType.TextDisplay => JsonSerializer.Deserialize(ref reader, Serialization.Default.JsonTextDisplayComponent),
                ComponentType.Thumbnail => JsonSerializer.Deserialize(ref reader, Serialization.Default.JsonThumbnailComponent),
                ComponentType.MediaGallery => JsonSerializer.Deserialize(ref reader, Serialization.Default.JsonMediaGalleryComponent),
                ComponentType.File => JsonSerializer.Deserialize(ref reader, Serialization.Default.JsonFileComponent),
                ComponentType.Separator => JsonSerializer.Deserialize(ref reader, Serialization.Default.JsonSeparatorComponent),
                ComponentType.Container => JsonSerializer.Deserialize(ref reader, Serialization.Default.JsonContainerComponent),
                ComponentType.Label => JsonSerializer.Deserialize(ref reader, Serialization.Default.JsonLabelComponent),
                ComponentType.FileUpload => JsonSerializer.Deserialize(ref reader, Serialization.Default.JsonFileUploadComponent),
                ComponentType.RadioGroup => JsonSerializer.Deserialize(ref reader, Serialization.Default.JsonRadioGroupComponent),
                ComponentType.CheckboxGroup => JsonSerializer.Deserialize(ref reader, Serialization.Default.JsonCheckboxGroupComponent),
                ComponentType.Checkbox => JsonSerializer.Deserialize(ref reader, Serialization.Default.JsonCheckboxComponent),
                _ => JsonSerializer.Deserialize(ref reader, Serialization.Default.JsonUnknownComponent),
            };

            [DoesNotReturn]
            [StackTraceHidden]
            static void ThrowMissingTypeProperty()
            {
                throw new JsonException("Missing property 'type'.");
            }
        }

        public override void Write(Utf8JsonWriter writer, JsonComponent value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}

[JsonGuard]
public partial class JsonActionRowComponent : JsonComponent
{
    [JsonPropertyName("components")]
    public JsonComponent[] Components { get; set; }
}

public partial class JsonButtonComponent : JsonComponent
{
    [JsonPropertyName("style")]
    public ButtonStyle Style { get; set; }

    [JsonPropertyName("label")]
    public string? Label { get; set; }

    [JsonPropertyName("emoji")]
    public JsonEmoji? Emoji { get; set; }

    [JsonPropertyName("custom_id")]
    public string? CustomId { get; set; }

    [JsonPropertyName("sku_id")]
    public ulong? SkuId { get; set; }

    [JsonPropertyName("url")]
    public string? Url { get; set; }

    [JsonPropertyName("disabled")]
    public bool? Disabled { get; set; }
}

[JsonGuard]
public partial class JsonSelectComponent : JsonComponent
{
    [JsonPropertyName("custom_id")]
    public string CustomId { get; set; }

    [JsonPropertyName("placeholder")]
    public string? Placeholder { get; set; }

    [JsonPropertyName("min_values")]
    public int? MinValues { get; set; }

    [JsonPropertyName("max_values")]
    public int? MaxValues { get; set; }

    [JsonPropertyName("required")]
    public bool? Required { get; set; }

    [JsonPropertyName("disabled")]
    public bool? Disabled { get; set; }
}

[JsonGuard]
public partial class JsonStringSelectComponent : JsonSelectComponent
{
    [JsonPropertyName("options")]
    public JsonStringSelectOption[] Options { get; set; }
}

[JsonGuard]
public partial class JsonTextInputComponent : JsonComponent
{
    [JsonPropertyName("custom_id")]
    public string CustomId { get; set; }

    [JsonPropertyName("style")]
    public TextInputStyle Style { get; set; }

    [JsonPropertyName("min_length")]
    public int? MinLength { get; set; }

    [JsonPropertyName("max_length")]
    public int? MaxLength { get; set; }

    [JsonPropertyName("required")]
    public bool? Required { get; set; }

    [JsonPropertyName("value")]
    public string? Value { get; set; }

    [JsonPropertyName("placeholder")]
    public string? Placeholder { get; set; }
}

public abstract class JsonEntitySelectComponent : JsonSelectComponent
{
    [JsonPropertyName("default_values")]
    public JsonEntitySelectDefaultValue[]? DefaultValues { get; set; }
}

[JsonGuard]
public partial class JsonUserSelectComponent : JsonEntitySelectComponent
{
}

[JsonGuard]
public partial class JsonRoleSelectComponent : JsonEntitySelectComponent
{
}

[JsonGuard]
public partial class JsonMentionableSelectComponent : JsonEntitySelectComponent
{
}

[JsonGuard]
public partial class JsonChannelSelectComponent : JsonEntitySelectComponent
{
    [JsonPropertyName("channel_types")]
    public ChannelType[]? ChannelTypes { get; set; }
}

[JsonGuard]
public partial class JsonComponentSectionComponent : JsonComponent
{
    [JsonPropertyName("components")]
    public JsonComponent[] Components { get; set; }

    [JsonPropertyName("accessory")]
    public JsonComponent Accessory { get; set; }
}

[JsonGuard]
public partial class JsonTextDisplayComponent : JsonComponent
{
    [JsonPropertyName("content")]
    public string Content { get; set; }
}

[JsonGuard]
public partial class JsonThumbnailComponent : JsonComponent
{
    [JsonPropertyName("media")]
    public JsonComponentMedia Media { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("spoiler")]
    public bool? Spoiler { get; set; }
}

[JsonGuard]
public partial class JsonMediaGalleryComponent : JsonComponent
{
    [JsonPropertyName("items")]
    public JsonMediaGalleryItem[] Items { get; set; }
}

[JsonGuard]
public partial class JsonMediaGalleryItem
{
    [JsonPropertyName("media")]
    public JsonComponentMedia Media { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("spoiler")]
    public bool? Spoiler { get; set; }
}

[JsonGuard]
public partial class JsonFileComponent : JsonComponent
{
    [JsonPropertyName("file")]
    public JsonComponentMedia File { get; set; }

    [JsonPropertyName("spoiler")]
    public bool? Spoiler { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("size")]
    public int? Size { get; set; }
}

public partial class JsonSeparatorComponent : JsonComponent
{
    [JsonPropertyName("divider")]
    public bool? Divider { get; set; }

    [JsonPropertyName("spacing")]
    public SeparatorSpacingSize? Spacing { get; set; }
}

[JsonGuard]
public partial class JsonContainerComponent : JsonComponent
{
    [JsonPropertyName("components")]
    public JsonComponent[] Components { get; set; }

    [JsonPropertyName("accent_color")]
    public Color? AccentColor { get; set; }

    [JsonPropertyName("spoiler")]
    public bool? Spoiler { get; set; }
}

[JsonGuard]
public partial class JsonLabelComponent : JsonComponent
{
    [JsonPropertyName("label")]
    public string Label { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("component")]
    public JsonComponent Component { get; set; }
}

[JsonGuard]
public partial class JsonFileUploadComponent : JsonComponent
{
    [JsonPropertyName("custom_id")]
    public string CustomId { get; set; }

    [JsonPropertyName("min_values")]
    public int? MinValues { get; set; }

    [JsonPropertyName("max_values")]
    public int? MaxValues { get; set; }

    [JsonPropertyName("required")]
    public bool? Required { get; set; }

    [JsonPropertyName("file_types")]
    public string[]? FileTypes { get; set; }
}

[JsonGuard]
public partial class JsonRadioGroupComponent : JsonComponent
{
    [JsonPropertyName("custom_id")]
    public string CustomId { get; set; }

    [JsonPropertyName("options")]
    public JsonRadioGroupOption[] Options { get; set; }

    [JsonPropertyName("required")]
    public bool? Required { get; set; }
}

[JsonGuard]
public partial class JsonRadioGroupOption
{
    [JsonPropertyName("value")]
    public string Value { get; set; }

    [JsonPropertyName("label")]
    public string Label { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("default")]
    public bool? Default { get; set; }
}

[JsonGuard]
public partial class JsonCheckboxGroupComponent : JsonComponent
{
    [JsonPropertyName("custom_id")]
    public string CustomId { get; set; }

    [JsonPropertyName("options")]
    public JsonCheckboxGroupOption[] Options { get; set; }

    [JsonPropertyName("min_values")]
    public int? MinValues { get; set; }

    [JsonPropertyName("max_values")]
    public int? MaxValues { get; set; }

    [JsonPropertyName("required")]
    public bool? Required { get; set; }
}

[JsonGuard]
public partial class JsonCheckboxGroupOption
{
    [JsonPropertyName("value")]
    public string Value { get; set; }

    [JsonPropertyName("label")]
    public string Label { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("default")]
    public bool? Default { get; set; }
}

[JsonGuard]
public partial class JsonCheckboxComponent : JsonComponent
{
    [JsonPropertyName("custom_id")]
    public string CustomId { get; set; }

    [JsonPropertyName("default")]
    public bool? Default { get; set; }
}
