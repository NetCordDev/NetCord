using System.Text.Json.Serialization;

namespace NetCord.Rest;

public static class InteractionCallback
{
    /// <summary>
    /// ACK a ping interaction.
    /// </summary>
    public static InteractionCallbackProperties Pong
        => new(InteractionCallbackType.Pong);

    /// <summary>
    /// Respond to an interaction with a <paramref name="message"/>.
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    public static InteractionCallbackProperties<InteractionMessageProperties> Message(InteractionMessageProperties message)
        => new(InteractionCallbackType.Message, message);

    /// <summary>
    /// ACK an interaction and modify a response later, the user sees a loading state.
    /// </summary>
    public static InteractionCallbackProperties<InteractionMessageProperties> DeferredMessage(MessageFlags? flags = null)
        => new(InteractionCallbackType.DeferredMessage, new() { Flags = flags });

    /// <summary>
    /// For components, ACK an interaction and modify the original message later; the user does not see a loading state.
    /// </summary>
    public static InteractionCallbackProperties DeferredModifyMessage
        => new(InteractionCallbackType.DeferredModifyMessage);

    /// <summary>
    /// For components, modify the message the component was attached to.
    /// </summary>
    /// <param name="action"></param>
    /// <returns></returns>
    public static InteractionCallbackProperties<MessageOptions> ModifyMessage(Action<MessageOptions> action)
    {
        MessageOptions options = new();
        action(options);
        return new(InteractionCallbackType.ModifyMessage, options);
    }

    /// <summary>
    /// Respond to an autocomplete interaction with suggested <paramref name="choices"/>.
    /// </summary>
    /// <param name="choices"></param>
    /// <returns></returns>
    public static InteractionCallbackProperties<InteractionCallbackChoicesDataProperties> Autocomplete(IEnumerable<ApplicationCommandOptionChoiceProperties>? choices)
        => new(InteractionCallbackType.Autocomplete, new(choices));

    /// <summary>
    /// Respond to an interaction with a popup <paramref name="modal"/>.
    /// </summary>
    /// <param name="modal"></param>
    /// <returns></returns>
    public static InteractionCallbackProperties<ModalProperties> Modal(ModalProperties modal)
        => new(InteractionCallbackType.Modal, modal);

    /// <summary>
    /// Launch the Activity associated with the app. Only available for apps with Activities enabled.
    /// </summary>
    public static InteractionCallbackProperties LaunchActivity
        => new(InteractionCallbackType.LaunchActivity);
}

public partial class InteractionCallbackProperties : IHttpSerializable
{
    [JsonPropertyName("type")]
    public InteractionCallbackType Type { get; }

    internal InteractionCallbackProperties(InteractionCallbackType type)
    {
        Type = type;
    }

    public HttpContent Serialize()
    {
        return this switch
        {
            InteractionCallbackProperties<InteractionMessageProperties> interactionCallback => IMessageProperties.Serialize(
                interactionCallback,
                Serialization.Default.InteractionCallbackPropertiesInteractionMessageProperties,
                interactionCallback.Data.Attachments),

            InteractionCallbackProperties<MessageOptions> interactionCallback => IMessageProperties.Serialize(
                interactionCallback,
                Serialization.Default.InteractionCallbackPropertiesMessageOptions,
                interactionCallback.Data.Attachments),

            InteractionCallbackProperties<InteractionCallbackChoicesDataProperties> interactionCallback => new JsonContent<InteractionCallbackProperties<InteractionCallbackChoicesDataProperties>>(
                interactionCallback,
                Serialization.Default.InteractionCallbackPropertiesInteractionCallbackChoicesDataProperties),

            InteractionCallbackProperties<ModalProperties> interactionCallback => new JsonContent<IInteractionCallbackProperties<IModalProperties>>(
                interactionCallback,
                Serialization.Default.IInteractionCallbackPropertiesIModalProperties),

            _ => new JsonContent<InteractionCallbackProperties>(
                this,
                Serialization.Default.InteractionCallbackProperties),
        };
    }
}

public partial class InteractionCallbackProperties<T> : InteractionCallbackProperties, IInteractionCallbackProperties<T>
{
    internal InteractionCallbackProperties(InteractionCallbackType type, T data) : base(type)
    {
        Data = data;
    }

    [JsonPropertyName("data")]
    public T Data { get; }
}

internal interface IInteractionCallbackProperties<out T>
{
    [JsonPropertyName("type")]
    public InteractionCallbackType Type { get; }

    [JsonPropertyName("data")]
    public T Data { get; }
}
