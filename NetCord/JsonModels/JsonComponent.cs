using System.Text.Json;
using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

[JsonConverter(typeof(JsonComponentConverter))]
public class JsonComponent
{
    [JsonPropertyName("type")]
    public ComponentType Type { get; set; }

    [JsonPropertyName("id")]
    public int Id { get; set; }

    public class JsonComponentConverter : JsonConverter<JsonComponent>
    {
        internal struct JsonComponentInternal
        {
            [JsonPropertyName("type")]
            public ComponentType Type { get; set; }

            [JsonPropertyName("id")]
            public int Id { get; set; }
        }

        public override JsonComponent? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var readerCopy = reader;

            while (true)
            {
                if (!readerCopy.Read())
                    throw new JsonException("Failed to read the next JSON token.");

                if (readerCopy.TokenType is JsonTokenType.PropertyName)
                {
                    if (readerCopy.ValueTextEquals("type"u8))
                        break;
                    else
                    {
                        readerCopy.Skip();
                        continue;
                    }
                }

                if (readerCopy.TokenType is JsonTokenType.EndObject)
                    throw new JsonException("Could not find a 'type' property.");
            }

            if (!readerCopy.Read())
                throw new JsonException("Failed to read the 'type' property value.");

            var type = (ComponentType)readerCopy.GetInt32();

            return type switch
            {
                ComponentType.ActionRow => JsonSerializer.Deserialize(ref reader, Serialization.Default.JsonActionRowComponent),
                ComponentType.Button => JsonSerializer.Deserialize(ref reader, Serialization.Default.JsonButtonComponent),
                ComponentType.StringMenu => JsonSerializer.Deserialize(ref reader, Serialization.Default.JsonStringMenuComponent),
                ComponentType.TextInput => JsonSerializer.Deserialize(ref reader, Serialization.Default.JsonTextInputComponent),
                ComponentType.UserMenu => JsonSerializer.Deserialize(ref reader, Serialization.Default.JsonUserMenuComponent),
                ComponentType.RoleMenu => JsonSerializer.Deserialize(ref reader, Serialization.Default.JsonRoleMenuComponent),
                ComponentType.MentionableMenu => JsonSerializer.Deserialize(ref reader, Serialization.Default.JsonMentionableMenuComponent),
                ComponentType.ChannelMenu => JsonSerializer.Deserialize(ref reader, Serialization.Default.JsonChannelMenuComponent),
                ComponentType.Section => JsonSerializer.Deserialize(ref reader, Serialization.Default.JsonComponentSectionComponent),
                ComponentType.TextDisplay => JsonSerializer.Deserialize(ref reader, Serialization.Default.JsonTextDisplayComponent),
                ComponentType.Thumbnail => JsonSerializer.Deserialize(ref reader, Serialization.Default.JsonThumbnailComponent),
                ComponentType.MediaGallery => JsonSerializer.Deserialize(ref reader, Serialization.Default.JsonMediaGalleryComponent),
                ComponentType.File => JsonSerializer.Deserialize(ref reader, Serialization.Default.JsonFileDisplayComponent),
                ComponentType.Separator => JsonSerializer.Deserialize(ref reader, Serialization.Default.JsonComponentSeparatorComponent),
                ComponentType.Container => JsonSerializer.Deserialize(ref reader, Serialization.Default.JsonComponentContainerComponent),
                ComponentType.Label => JsonSerializer.Deserialize(ref reader, Serialization.Default.JsonLabelComponent),
                ComponentType.FileUpload => JsonSerializer.Deserialize(ref reader, Serialization.Default.JsonFileUploadComponent),
                ComponentType.RadioGroup => JsonSerializer.Deserialize(ref reader, Serialization.Default.JsonRadioGroupComponent),
                ComponentType.CheckboxGroup => JsonSerializer.Deserialize(ref reader, Serialization.Default.JsonCheckboxGroupComponent),
                ComponentType.Checkbox => JsonSerializer.Deserialize(ref reader, Serialization.Default.JsonCheckboxComponent),
                _ => DeserializeUnknown(ref reader),
            };

            static JsonComponent DeserializeUnknown(ref Utf8JsonReader reader)
            {
                var component = JsonSerializer.Deserialize(ref reader, Serialization.Default.JsonComponentInternal);
                return new()
                {
                    Type = component.Type,
                    Id = component.Id,
                };
            }
        }

        public override void Write(Utf8JsonWriter writer, JsonComponent value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}

public class JsonActionRowComponent : JsonComponent
{
    [JsonPropertyName("components")]
    public JsonComponent[] Components { get; set; }
}

public class JsonButtonComponent : JsonComponent
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

public class JsonMenuComponent : JsonComponent
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

public class JsonStringMenuComponent : JsonMenuComponent
{
    [JsonPropertyName("options")]
    public JsonStringMenuSelectOption[]? Options { get; set; }

    [JsonPropertyName("values")]
    public string[]? SelectedValues { get; set; }
}

public class JsonTextInputComponent : JsonComponent
{
    [JsonPropertyName("custom_id")]
    public string CustomId { get; set; }

    [JsonPropertyName("value")]
    public string Value { get; set; }
}

public abstract class JsonEntityMenuComponent : JsonMenuComponent
{
    [JsonPropertyName("default_values")]
    public JsonEntityMenuDefaultValue[]? DefaultValues { get; set; }

    [JsonPropertyName("values")]
    public ulong[]? SelectedValues { get; set; }
}

public class JsonUserMenuComponent : JsonEntityMenuComponent
{
}

public class JsonRoleMenuComponent : JsonEntityMenuComponent
{
}

public class JsonMentionableMenuComponent : JsonEntityMenuComponent
{
}

public class JsonChannelMenuComponent : JsonEntityMenuComponent
{
    [JsonPropertyName("channel_types")]
    public ChannelType[]? ChannelTypes { get; set; }
}

public class JsonComponentSectionComponent : JsonComponent
{
    [JsonPropertyName("components")]
    public JsonComponent[] Components { get; set; }

    [JsonPropertyName("accessory")]
    public JsonComponent Accessory { get; set; }
}

public class JsonThumbnailComponent : JsonComponent
{
    [JsonPropertyName("media")]
    public JsonComponentMedia Media { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("spoiler")]
    public bool? Spoiler { get; set; }
}

public class JsonTextDisplayComponent : JsonComponent
{
    [JsonPropertyName("content")]
    public string? Content { get; set; }
}

public class JsonMediaGalleryComponent : JsonComponent
{
    [JsonPropertyName("items")]
    public JsonMediaGalleryItem[] Items { get; set; }
}

public class JsonMediaGalleryItem
{
    [JsonPropertyName("media")]
    public JsonComponentMedia Media { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("spoiler")]
    public bool? Spoiler { get; set; }
}

public class JsonFileDisplayComponent : JsonComponent
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

public class JsonComponentSeparatorComponent : JsonComponent
{
    [JsonPropertyName("divider")]
    public bool? Divider { get; set; }

    [JsonPropertyName("spacing")]
    public ComponentSeparatorSpacingSize? Spacing { get; set; }
}

public class JsonComponentContainerComponent : JsonComponent
{
    [JsonPropertyName("components")]
    public JsonComponent[] Components { get; set; }

    [JsonPropertyName("accent_color")]
    public Color? AccentColor { get; set; }

    [JsonPropertyName("spoiler")]
    public bool? Spoiler { get; set; }
}

public class JsonLabelComponent : JsonComponent
{
    [JsonPropertyName("component")]
    public JsonComponent Component { get; set; }
}

public class JsonFileUploadComponent : JsonComponent
{
    [JsonPropertyName("custom_id")]
    public string CustomId { get; set; }

    [JsonPropertyName("values")]
    public ulong[] Values { get; set; }
}

public class JsonRadioGroupComponent : JsonComponent
{
    [JsonPropertyName("custom_id")]
    public string CustomId { get; set; }

    [JsonPropertyName("value")]
    public string? SelectedValue { get; set; }
}

public class JsonCheckboxGroupComponent : JsonComponent
{
    [JsonPropertyName("custom_id")]
    public string CustomId { get; set; }

    [JsonPropertyName("values")]
    public string[] SelectedValues { get; set; }
}

public class JsonCheckboxComponent : JsonComponent
{
    [JsonPropertyName("custom_id")]
    public string CustomId { get; set; }

    [JsonPropertyName("value")]
    public bool Selected { get; set; }
}
