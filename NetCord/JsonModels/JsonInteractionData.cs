using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;

using NetCord.JsonConverters;

using JsonGuard;

namespace NetCord.JsonModels;

[JsonGuard]
public partial class JsonApplicationCommandInteractionData
{
    [JsonPropertyName("id")]
    public ulong Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("type")]
    public ApplicationCommandType Type { get; set; }

    [JsonPropertyName("resolved")]
    public JsonInteractionResolvedData? Resolved { get; set; }

    [JsonPropertyName("options")]
    public JsonApplicationCommandInteractionDataOption[]? Options { get; set; }

    [JsonPropertyName("guild_id")]
    public ulong? GuildId { get; set; }

    [JsonPropertyName("target_id")]
    public ulong? TargetId { get; set; }
}

public abstract class JsonComponentInteractionData
{
#pragma warning disable CS8618
    [JsonPropertyName("custom_id")]
    public string CustomId { get; set; }
#pragma warning restore CS8618

    [JsonPropertyName("resolved")]
    public JsonInteractionResolvedData? Resolved { get; set; }
}

[JsonConverter(typeof(Converter))]
public abstract class JsonMessageComponentInteractionData : JsonComponentInteractionData
{
    [JsonPropertyName("component_type")]
    public ComponentType Type { get; set; }

    [JsonPropertyName("id")]
    public int Id { get; set; }

    public class Converter : JsonConverter<JsonMessageComponentInteractionData>
    {
        public override JsonMessageComponentInteractionData? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var readerCopy = reader;

            if (!JsonConverterHelper.TrySkipToProperty(ref readerCopy, "component_type"u8))
                ThrowMissingComponentTypeProperty();

            return (ComponentType)readerCopy.GetInt32() switch
            {
                ComponentType.Button => JsonSerializer.Deserialize(ref reader, Serialization.Default.JsonButtonInteractionData),
                ComponentType.StringSelect => JsonSerializer.Deserialize(ref reader, Serialization.Default.JsonStringSelectInteractionData),
                ComponentType.UserSelect => JsonSerializer.Deserialize(ref reader, Serialization.Default.JsonUserSelectInteractionData),
                ComponentType.RoleSelect => JsonSerializer.Deserialize(ref reader, Serialization.Default.JsonRoleSelectInteractionData),
                ComponentType.MentionableSelect => JsonSerializer.Deserialize(ref reader, Serialization.Default.JsonMentionableSelectInteractionData),
                ComponentType.ChannelSelect => JsonSerializer.Deserialize(ref reader, Serialization.Default.JsonChannelSelectInteractionData),
                _ => JsonSerializer.Deserialize(ref reader, Serialization.Default.JsonUnknownComponentInteractionData),
            };

            [DoesNotReturn]
            [StackTraceHidden]
            static void ThrowMissingComponentTypeProperty() => throw new JsonException("Missing property 'component_type'.");
        }

        public override void Write(Utf8JsonWriter writer, JsonMessageComponentInteractionData value, JsonSerializerOptions options) => throw new NotSupportedException();
    }
}

[JsonGuard]
public partial class JsonModalSubmitInteractionData : JsonComponentInteractionData
{
    [JsonPropertyName("components")]
    public JsonComponentData[] Components { get; set; }
}

[JsonConverter(typeof(Converter))]
public abstract class JsonComponentData
{
    [JsonPropertyName("type")]
    public virtual ComponentType Type { get; set; }

    [JsonPropertyName("id")]
    public int Id { get; set; }

    public class Converter : JsonConverter<JsonComponentData>
    {
        public override JsonComponentData? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var readerCopy = reader;

            if (!JsonConverterHelper.TrySkipToProperty(ref readerCopy, "type"u8))
                ThrowMissingTypeProperty();

            return (ComponentType)readerCopy.GetInt32() switch
            {
                ComponentType.StringSelect => JsonSerializer.Deserialize(ref reader, Serialization.Default.JsonStringSelectComponentData),
                ComponentType.TextInput => JsonSerializer.Deserialize(ref reader, Serialization.Default.JsonTextInputComponentData),
                ComponentType.UserSelect => JsonSerializer.Deserialize(ref reader, Serialization.Default.JsonUserSelectComponentData),
                ComponentType.RoleSelect => JsonSerializer.Deserialize(ref reader, Serialization.Default.JsonRoleSelectComponentData),
                ComponentType.MentionableSelect => JsonSerializer.Deserialize(ref reader, Serialization.Default.JsonMentionableSelectComponentData),
                ComponentType.ChannelSelect => JsonSerializer.Deserialize(ref reader, Serialization.Default.JsonChannelSelectComponentData),
                ComponentType.TextDisplay => JsonSerializer.Deserialize(ref reader, Serialization.Default.JsonTextDisplayComponentData),
                ComponentType.Label => JsonSerializer.Deserialize(ref reader, Serialization.Default.JsonLabelComponentData),
                ComponentType.FileUpload => JsonSerializer.Deserialize(ref reader, Serialization.Default.JsonFileUploadComponentData),
                ComponentType.RadioGroup => JsonSerializer.Deserialize(ref reader, Serialization.Default.JsonRadioGroupComponentData),
                ComponentType.CheckboxGroup => JsonSerializer.Deserialize(ref reader, Serialization.Default.JsonCheckboxGroupComponentData),
                ComponentType.Checkbox => JsonSerializer.Deserialize(ref reader, Serialization.Default.JsonCheckboxComponentData),
                _ => JsonSerializer.Deserialize(ref reader, Serialization.Default.JsonUnknownComponentData),
            };

            [DoesNotReturn]
            [StackTraceHidden]
            static void ThrowMissingTypeProperty() => throw new JsonException("Missing property 'type'.");
        }

        public override void Write(Utf8JsonWriter writer, JsonComponentData value, JsonSerializerOptions options) => throw new NotSupportedException();
    }
}

[JsonGuard]
public partial class JsonButtonComponentData : JsonComponentData
{
    [JsonPropertyName("custom_id")]
    public string CustomId { get; set; }
}

[JsonGuard]
public partial class JsonStringSelectComponentData : JsonComponentData
{
    [JsonPropertyName("custom_id")]
    public string CustomId { get; set; }

    [JsonPropertyName("values")]
    public string[] Values { get; set; }
}

[JsonGuard]
public partial class JsonTextInputComponentData : JsonComponentData
{
    [JsonPropertyName("custom_id")]
    public string CustomId { get; set; }

    [JsonPropertyName("value")]
    public string Value { get; set; }
}

public abstract class JsonEntitySelectComponentData : JsonComponentData
{
#pragma warning disable CS8618
    [JsonPropertyName("custom_id")]
    public string CustomId { get; set; }
#pragma warning restore CS8618

#pragma warning disable CS8618
    [JsonPropertyName("values")]
    public ulong[] Values { get; set; }
#pragma warning restore CS8618
}

[JsonGuard]
public partial class JsonUserSelectComponentData : JsonEntitySelectComponentData;

[JsonGuard]
public partial class JsonRoleSelectComponentData : JsonEntitySelectComponentData;

[JsonGuard]
public partial class JsonMentionableSelectComponentData : JsonEntitySelectComponentData;

[JsonGuard]
public partial class JsonChannelSelectComponentData : JsonEntitySelectComponentData;

public class JsonTextDisplayComponentData : JsonComponentData;

[JsonGuard]
public partial class JsonLabelComponentData : JsonComponentData
{
    [JsonPropertyName("component")]
    public JsonComponentData Component { get; set; }
}

[JsonGuard]
public partial class JsonFileUploadComponentData : JsonComponentData
{
    [JsonPropertyName("custom_id")]
    public string CustomId { get; set; }

    [JsonPropertyName("values")]
    public ulong[] Values { get; set; }
}

[JsonGuard]
public partial class JsonRadioGroupComponentData : JsonComponentData
{
    [JsonPropertyName("custom_id")]
    public string CustomId { get; set; }

    [JsonPropertyName("value")]
    public string? Value { get; set; }
}

[JsonGuard]
public partial class JsonCheckboxGroupComponentData : JsonComponentData
{
    [JsonPropertyName("custom_id")]
    public string CustomId { get; set; }

    [JsonPropertyName("values")]
    public string[] Values { get; set; }
}

[JsonGuard]
public partial class JsonCheckboxComponentData : JsonComponentData
{
    [JsonPropertyName("custom_id")]
    public string CustomId { get; set; }

    [JsonPropertyName("value")]
    public bool Value { get; set; }
}

public class JsonUnknownComponentData : JsonComponentData;

[JsonGuard]
public partial class JsonButtonInteractionData : JsonMessageComponentInteractionData;

[JsonGuard]
public partial class JsonStringSelectInteractionData : JsonMessageComponentInteractionData
{
    [JsonPropertyName("values")]
    public string[] Values { get; set; }
}

[JsonGuard]
public partial class JsonEntitySelectInteractionData : JsonMessageComponentInteractionData
{
    [JsonPropertyName("values")]
    public ulong[] Values { get; set; }
}

[JsonGuard]
public partial class JsonUserSelectInteractionData : JsonEntitySelectInteractionData;

[JsonGuard]
public partial class JsonRoleSelectInteractionData : JsonEntitySelectInteractionData;

[JsonGuard]
public partial class JsonMentionableSelectInteractionData : JsonEntitySelectInteractionData;

[JsonGuard]
public partial class JsonChannelSelectInteractionData : JsonEntitySelectInteractionData;

[JsonGuard]
public partial class JsonUnknownComponentInteractionData : JsonMessageComponentInteractionData;
