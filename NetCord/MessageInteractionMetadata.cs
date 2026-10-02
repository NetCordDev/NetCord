using NetCord.JsonModels;
using NetCord.Rest;

namespace NetCord;

public class MessageInteractionMetadata(JsonMessageInteractionMetadata jsonModel, RestClient client) : Entity
{
    public static MessageInteractionMetadata Create(JsonMessageInteractionMetadata jsonModel, RestClient client)
    {
        return jsonModel switch
        {
            JsonMessageApplicationCommandInteractionMetadata applicationCommand => new MessageApplicationCommandInteractionMetadata(applicationCommand, client),
            JsonMessageMessageComponentInteractionMetadata messageComponent => new MessageMessageComponentInteractionMetadata(messageComponent, client),
            JsonMessageModalSubmitInteractionMetadata modalSubmit => new MessageModalSubmitInteractionMetadata(modalSubmit, client),
            _ => new MessageUnknownInteractionMetadata(jsonModel, client),
        };
    }

    public override ulong Id { get; } = jsonModel.Id;

    /// <summary>
    /// Type of interaction.
    /// </summary>
    public InteractionType Type { get; } = jsonModel.Type;

    /// <summary>
    /// The user who triggered the interaction.
    /// </summary>
    public User User { get; } = new(jsonModel.User, client);

    /// <summary>
    /// IDs for installation context(s) related to an interaction.
    /// </summary>
    public IReadOnlyDictionary<ApplicationIntegrationType, ulong> AuthorizingIntegrationOwners { get; } = jsonModel.AuthorizingIntegrationOwners;

    /// <summary>
    /// ID of the original response message, present only on follow-up messages.
    /// </summary>
    public ulong? OriginalResponseMessageId { get; } = jsonModel.OriginalResponseMessageId;
}

public class MessageApplicationCommandInteractionMetadata(JsonMessageApplicationCommandInteractionMetadata jsonModel, RestClient client) : MessageInteractionMetadata(jsonModel, client)
{

    /// <summary>
    /// The user targeted by the command, present only on user commands.
    /// </summary>
    public User? TargetUser { get; } = jsonModel.TargetUser is { } targetUser ? new(targetUser, client) : null;

    /// <summary>
    /// The message targeted by the command, present only on message commands.
    /// </summary>
    public ulong? TargetMessageId { get; } = jsonModel.TargetMessageId;
}

public class MessageMessageComponentInteractionMetadata(JsonMessageMessageComponentInteractionMetadata jsonModel, RestClient client) : MessageInteractionMetadata(jsonModel, client)
{
    /// <summary>
    /// The message that contained the component that was interacted with.
    /// </summary>
    public ulong InteractedMessageId { get; } = jsonModel.InteractedMessageId;
}

public class MessageModalSubmitInteractionMetadata(JsonMessageModalSubmitInteractionMetadata jsonModel, RestClient client) : MessageInteractionMetadata(jsonModel, client)
{
    /// <summary>
    /// Metadata for the interaction that was used to open the modal.
    /// </summary>
    public MessageInteractionMetadata TriggeringInteractionMetadata { get; } = Create(jsonModel.TriggeringInteractionMetadata, client);
}

public class MessageUnknownInteractionMetadata(JsonMessageInteractionMetadata jsonModel, RestClient client) : MessageInteractionMetadata(jsonModel, client);
