using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;

using NetCord.JsonConverters;

using JsonGuard;

namespace NetCord.JsonModels;

[JsonConverter(typeof(Converter))]
public abstract class JsonMessageInteractionMetadata : JsonEntity
{
    [JsonPropertyName("type")]
    public InteractionType Type { get; set; }

#pragma warning disable CS8618
    [JsonPropertyName("user")]
    public JsonUser User { get; set; }
#pragma warning restore CS8618

#pragma warning disable CS8618
    [JsonPropertyName("authorizing_integration_owners")]
    public IReadOnlyDictionary<ApplicationIntegrationType, ulong> AuthorizingIntegrationOwners { get; set; }
#pragma warning restore CS8618

    [JsonPropertyName("original_response_message_id")]
    public ulong? OriginalResponseMessageId { get; set; }

    public class Converter : JsonConverter<JsonMessageInteractionMetadata>
    {
        public override JsonMessageInteractionMetadata? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var readerCopy = reader;

            if (!JsonConverterHelper.TrySkipToProperty(ref readerCopy, "type"u8))
                ThrowNoTypeProperty();

            return (InteractionType)readerCopy.GetInt32() switch
            {
                InteractionType.ApplicationCommand => JsonSerializer.Deserialize(ref reader, Serialization.Default.JsonMessageApplicationCommandInteractionMetadata),
                InteractionType.MessageComponent => JsonSerializer.Deserialize(ref reader, Serialization.Default.JsonMessageMessageComponentInteractionMetadata),
                InteractionType.ModalSubmit => JsonSerializer.Deserialize(ref reader, Serialization.Default.JsonMessageModalSubmitInteractionMetadata),
                _ => JsonSerializer.Deserialize(ref reader, Serialization.Default.JsonMessageUnknownInteractionMetadata),
            };

            [DoesNotReturn]
            [StackTraceHidden]
            static void ThrowNoTypeProperty()
            {
                throw new JsonException("No 'type' property found.");
            }
        }

        public override void Write(Utf8JsonWriter writer, JsonMessageInteractionMetadata value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}

[JsonGuard]
public partial class JsonMessageApplicationCommandInteractionMetadata : JsonMessageInteractionMetadata
{
    [JsonPropertyName("target_user")]
    public JsonUser? TargetUser { get; set; }

    [JsonPropertyName("target_message_id")]
    public ulong? TargetMessageId { get; set; }
}

[JsonGuard]
public partial class JsonMessageMessageComponentInteractionMetadata : JsonMessageInteractionMetadata
{
    [JsonPropertyName("interacted_message_id")]
    public ulong InteractedMessageId { get; set; }
}

[JsonGuard]
public partial class JsonMessageModalSubmitInteractionMetadata : JsonMessageInteractionMetadata
{
    [JsonPropertyName("triggering_interaction_metadata")]
    public JsonMessageInteractionMetadata TriggeringInteractionMetadata { get; set; }
}

[JsonGuard]
public partial class JsonMessageUnknownInteractionMetadata : JsonMessageInteractionMetadata;
