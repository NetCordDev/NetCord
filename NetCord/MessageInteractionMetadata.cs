using NetCord.JsonModels;
using NetCord.Rest;

namespace NetCord;

/// <summary>
/// Represents metadata about an interaction associated with a message.
/// </summary>
public class MessageInteractionMetadata(JsonMessageInteractionMetadata jsonModel, RestClient client) : Entity, IJsonModel<JsonMessageInteractionMetadata>
{
    JsonMessageInteractionMetadata IJsonModel<JsonMessageInteractionMetadata>.JsonModel => jsonModel;

    /// <summary>
    /// The unique identifier of the interaction.
    /// </summary>
    public override ulong Id => jsonModel.Id;

    /// <summary>
    /// Type of interaction.
    /// </summary>
    public InteractionType Type => jsonModel.Type;

    /// <summary>
    /// The user who triggered the interaction.
    /// </summary>
    public User User { get; } = new(jsonModel.User, client);

    /// <summary>
    /// IDs for installation context(s) related to an interaction.
    /// </summary>
    public IReadOnlyDictionary<ApplicationIntegrationType, ulong> AuthorizingIntegrationOwners => jsonModel.AuthorizingIntegrationOwners;

    /// <summary>
    /// ID of the original response message, present only on follow-up messages.
    /// </summary>
    public ulong? OriginalResponseMessageId => jsonModel.OriginalResponseMessageId;

    /// <summary>
    /// ID of the message that contained interactive component, present only on messages created from component interactions.
    /// </summary>
    public ulong? InteractedMessageId => jsonModel.InteractedMessageId;

    /// <summary>
    /// Metadata for the interaction that was used to open the modal, present only on modal interactions.
    /// </summary>
    public MessageInteractionMetadata? TriggeringInteractionMetadata { get; } = jsonModel.TriggeringInteractionMetadata is { } triggeringInteractionMetadata ? new(triggeringInteractionMetadata, client) : null;
}
