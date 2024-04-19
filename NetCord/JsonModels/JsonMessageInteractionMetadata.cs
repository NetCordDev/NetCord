using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public class JsonMessageInteractionMetadata : JsonEntity
{
    [JsonPropertyName("type")]
    public InteractionType Type { get; set; }

    [JsonPropertyName("user_id")]
    public ulong UserId { get; set; }

    [JsonPropertyName("authorizing_integration_owners")]
    public IReadOnlyDictionary<ApplicationIntegrationType, ulong> AuthorizingIntegrationOwners { get; set; }

    [JsonPropertyName("original_response_message_id")]
    public ulong? OriginalResponseMessageId { get; set; }

    [JsonPropertyName("interacted_message_id")]
    public ulong? InteractedMessageId { get; set; }

    [JsonPropertyName("triggering_interaction_metadata")]
    public JsonMessageInteractionMetadata? TriggeringInteractionMetadata { get; set; }
}
