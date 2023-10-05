namespace NetCord.Rest;

public enum InteractionCallbackType
{
    /// <summary>
    /// ACK a <see cref="InteractionType.Ping"/> interaction.
    /// </summary>
    Pong = 1,

    /// <summary>
    /// Respond to an interaction with a message.
    /// </summary>
    ChannelMessageWithSource = 4,

    /// <summary>
    /// ACK an interaction and modify a response later, the user sees a loading state.
    /// </summary>
    DeferredChannelMessageWithSource = 5,

    /// <summary>
    /// For components, ACK an interaction and modify the original message later; the user does not see a loading state.
    /// </summary>
    DeferredModifyMessage = 6,

    /// <summary>
    /// For components, modify the message the component was attached to.
    /// </summary>
    ModifyMessage = 7,

    /// <summary>
    /// Respond to an autocomplete interaction with suggested choices.
    /// </summary>
    ApplicationCommandAutocompleteResult = 8,

    /// <summary>
    /// Respond to an interaction with a popup modal.
    /// </summary>
    Modal = 9,

    /// <summary>
    /// Respond to an interaction with an upgrade button, only available for apps with monetization enabled.
    /// </summary>
    PremiumRequired = 10,
}
