using System.Text.Json.Serialization;

namespace NetCord.Rest;

public partial class InteractionCallback : IHttpSerializable
{
    [JsonPropertyName("type")]
    public InteractionCallbackType Type { get; }

    private protected InteractionCallback(InteractionCallbackType type)
    {
        Type = type;
    }

    /// <summary>
    /// ACK a ping interaction.
    /// </summary>
    public static InteractionCallback Pong
        => new(InteractionCallbackType.Pong);

    /// <summary>
    /// Respond to an interaction with a <paramref name="message"/>.
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    public static InteractionCallback<InteractionMessageProperties> Message(InteractionMessageProperties message)
        => new(InteractionCallbackType.Message, message);

    /// <summary>
    /// ACK an interaction and modify a response later, the user sees a loading state.
    /// </summary>
    public static InteractionCallback<InteractionMessageProperties> DeferredMessage(MessageFlags? flags = null)
        => new(InteractionCallbackType.DeferredMessage, new() { Flags = flags });

    /// <summary>
    /// For components, ACK an interaction and modify the original message later; the user does not see a loading state.
    /// </summary>
    public static InteractionCallback DeferredModifyMessage
        => new(InteractionCallbackType.DeferredModifyMessage);

    /// <summary>
    /// For components, modify the message the component was attached to.
    /// </summary>
    /// <param name="action"></param>
    /// <returns></returns>
    public static InteractionCallback<MessageOptions> ModifyMessage(Action<MessageOptions> action)
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
    public static InteractionCallback<InteractionCallbackChoicesDataProperties> Autocomplete(IEnumerable<ApplicationCommandOptionChoiceProperties>? choices)
        => new(InteractionCallbackType.Autocomplete, new(choices));

    /// <summary>
    /// Respond to an interaction with a popup <paramref name="modal"/>.
    /// </summary>
    /// <param name="modal"></param>
    /// <returns></returns>
    public static InteractionCallback<ModalProperties> Modal(ModalProperties modal)
        => new(InteractionCallbackType.Modal, modal);

    /// <summary>
    /// Launch the Activity associated with the app. Only available for apps with Activities enabled.
    /// </summary>
    public static InteractionCallback LaunchActivity
        => new(InteractionCallbackType.LaunchActivity);

    public HttpContent Serialize()
    {
        return this switch
        {
            InteractionCallback<InteractionMessageProperties> interactionCallback => IMessageProperties.Serialize(
                interactionCallback,
                Serialization.Default.InteractionCallbackInteractionMessageProperties,
                interactionCallback.Data.Attachments),

            InteractionCallback<MessageOptions> interactionCallback => IMessageProperties.Serialize(
                interactionCallback,
                Serialization.Default.InteractionCallbackMessageOptions,
                interactionCallback.Data.Attachments),

            InteractionCallback<InteractionCallbackChoicesDataProperties> interactionCallback => new JsonContent<InteractionCallback<InteractionCallbackChoicesDataProperties>>(
                interactionCallback,
                Serialization.Default.InteractionCallbackInteractionCallbackChoicesDataProperties),

            InteractionCallback<ModalProperties> interactionCallback => new JsonContent<IInteractionCallback<IModalProperties>>(
                interactionCallback,
                Serialization.Default.IInteractionCallbackIModalProperties),

            _ => new JsonContent<InteractionCallback>(
                this,
                Serialization.Default.InteractionCallback),
        };
    }
}

public class InteractionCallback<T> : InteractionCallback, IInteractionCallback<T>
{
    internal InteractionCallback(InteractionCallbackType type, T data) : base(type)
    {
        Data = data;
    }

    [JsonPropertyName("data")]
    public T Data { get; }
}

internal interface IInteractionCallback<out T>
{
    [JsonPropertyName("type")]
    public InteractionCallbackType Type { get; }

    [JsonPropertyName("data")]
    public T Data { get; }
}
