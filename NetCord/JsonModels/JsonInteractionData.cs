using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;

using NetCord.JsonConverters;

namespace NetCord.JsonModels;

public class JsonApplicationCommandInteractionData
{
    [JsonPropertyName("id")]
    public required ulong Id { get; set; }

    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("type")]
    public required ApplicationCommandType Type { get; set; }

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
    [JsonPropertyName("custom_id")]
    public required string CustomId { get; set; }

    [JsonPropertyName("resolved")]
    public JsonInteractionResolvedData? Resolved { get; set; }
}

[JsonConverter(typeof(Converter))]
public abstract class JsonMessageComponentInteractionData : JsonComponentInteractionData
{
    [JsonPropertyName("component_type")]
    public required ComponentType Type { get; set; }

    [JsonPropertyName("id")]
    public required int Id { get; set; }

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

public class JsonModalSubmitInteractionData : JsonComponentInteractionData
{
    [JsonPropertyName("components")]
    public required JsonComponentData[] Components { get; set; }
}

[JsonConverter(typeof(Converter))]
public abstract class JsonComponentData
{
    [JsonPropertyName("type")]
    public required virtual ComponentType Type { get; set; }

    [JsonPropertyName("id")]
    public required int Id { get; set; }

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

public class JsonButtonComponentData : JsonComponentData
{
    [JsonPropertyName("custom_id")]
    public required string CustomId { get; set; }
}

public class JsonStringSelectComponentData : JsonComponentData
{
    [JsonPropertyName("custom_id")]
    public required string CustomId { get; set; }

    [JsonPropertyName("values")]
    public required string[] Values { get; set; }
}

public class JsonTextInputComponentData : JsonComponentData
{
    [JsonPropertyName("custom_id")]
    public required string CustomId { get; set; }

    [JsonPropertyName("value")]
    public required string Value { get; set; }
}

public abstract class JsonEntitySelectComponentData : JsonComponentData
{
    [JsonPropertyName("custom_id")]
    public required string CustomId { get; set; }

    [JsonPropertyName("values")]
    public required ulong[] Values { get; set; }
}

public class JsonUserSelectComponentData : JsonEntitySelectComponentData;

public class JsonRoleSelectComponentData : JsonEntitySelectComponentData;

public class JsonMentionableSelectComponentData : JsonEntitySelectComponentData;

public class JsonChannelSelectComponentData : JsonEntitySelectComponentData;

public class JsonTextDisplayComponentData : JsonComponentData;

public class JsonLabelComponentData : JsonComponentData
{
    [JsonPropertyName("component")]
    public required JsonComponentData Component { get; set; }
}

public class JsonFileUploadComponentData : JsonComponentData
{
    [JsonPropertyName("custom_id")]
    public required string CustomId { get; set; }

    [JsonPropertyName("values")]
    public required ulong[] Values { get; set; }
}

public class JsonRadioGroupComponentData : JsonComponentData
{
    [JsonPropertyName("custom_id")]
    public required string CustomId { get; set; }

    [JsonPropertyName("value")]
    public required string? Value { get; set; }
}

public class JsonCheckboxGroupComponentData : JsonComponentData
{
    [JsonPropertyName("custom_id")]
    public required string CustomId { get; set; }

    [JsonPropertyName("values")]
    public required string[] Values { get; set; }
}

public class JsonCheckboxComponentData : JsonComponentData
{
    [JsonPropertyName("custom_id")]
    public required string CustomId { get; set; }

    [JsonPropertyName("value")]
    public required bool Value { get; set; }
}

public class JsonUnknownComponentData : JsonComponentData;

public class JsonButtonInteractionData : JsonMessageComponentInteractionData;

public class JsonStringSelectInteractionData : JsonMessageComponentInteractionData
{
    [JsonPropertyName("values")]
    public required string[] Values { get; set; }
}

public class JsonEntitySelectInteractionData : JsonMessageComponentInteractionData
{
    [JsonPropertyName("values")]
    public required ulong[] Values { get; set; }
}

public class JsonUserSelectInteractionData : JsonEntitySelectInteractionData;

public class JsonRoleSelectInteractionData : JsonEntitySelectInteractionData;

public class JsonMentionableSelectInteractionData : JsonEntitySelectInteractionData;

public class JsonChannelSelectInteractionData : JsonEntitySelectInteractionData;

public class JsonUnknownComponentInteractionData : JsonMessageComponentInteractionData;
