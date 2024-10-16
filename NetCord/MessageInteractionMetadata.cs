using NetCord.JsonModels;
using NetCord.Rest;

namespace NetCord;

public class MessageInteractionMetadata : Entity, IJsonModel<JsonMessageInteractionMetadata>
{
    public MessageInteractionMetadata(JsonMessageInteractionMetadata jsonModel, RestClient client)
    {
        _jsonModel = jsonModel;

        User = new(jsonModel.User, client);

        var triggeringInteractionMetadata = jsonModel.TriggeringInteractionMetadata;
        if (triggeringInteractionMetadata is not null)
            TriggeringInteractionMetadata = new(triggeringInteractionMetadata, client);
    }

    private readonly JsonMessageInteractionMetadata _jsonModel;
    JsonMessageInteractionMetadata IJsonModel<JsonMessageInteractionMetadata>.JsonModel => _jsonModel;

    public override ulong Id => _jsonModel.Id;

    /// <summary>
    /// Type of interaction.
    /// </summary>
    public InteractionType Type => _jsonModel.Type;

    /// <summary>
    /// ID of the user who triggered the interaction
    /// </summary>
    public User User { get; }

    /// <summary>
    /// IDs for installation context(s) related to an interaction.
    /// </summary>
    public IReadOnlyDictionary<ApplicationIntegrationType, ulong> AuthorizingIntegrationOwners => _jsonModel.AuthorizingIntegrationOwners;

    /// <summary>
    /// ID of the original response message, present only on follow-up messages.
    /// </summary>
    public ulong? OriginalResponseMessageId => _jsonModel.OriginalResponseMessageId;

    /// <summary>
    /// ID of the message that contained interactive component, present only on messages created from component interactions.
    /// </summary>
    public ulong? InteractedMessageId => _jsonModel.InteractedMessageId;

    /// <summary>
    /// Metadata for the interaction that was used to open the modal, present only on modal interactions.
    /// </summary>
    public MessageInteractionMetadata? TriggeringInteractionMetadata { get; }
}
