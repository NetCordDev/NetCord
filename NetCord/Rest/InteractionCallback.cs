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
    /// Respond to an interaction with an upgrade button, only available for apps with monetization enabled.
    /// </summary>
    public static InteractionCallback PremiumRequired
        => new(InteractionCallbackType.PremiumRequired);

    public HttpContent Serialize()
    {
        switch (this)
        {
            case InteractionCallback<InteractionMessageProperties> interactionCallback:
                MultipartFormDataContent content = new()
                {
                    { new JsonContent<InteractionCallback<InteractionMessageProperties>>(interactionCallback, InteractionCallbackOfInteractionMessagePropertiesSerializerContext.WithOptions.InteractionCallbackInteractionMessageProperties), "payload_json" },
                };
                AttachmentProperties.AddAttachments(content, interactionCallback.Data.Attachments);
                return content;

            case InteractionCallback<MessageOptions> interactionCallback:
                content = new()
                {
                    { new JsonContent<InteractionCallback<MessageOptions>>(interactionCallback, InteractionCallbackOfMessageOptionsSerializerContext.WithOptions.InteractionCallbackMessageOptions), "payload_json" },
                };
                AttachmentProperties.AddAttachments(content, interactionCallback.Data.Attachments);
                return content;

            case InteractionCallback<InteractionCallbackChoicesDataProperties> interactionCallback:
                return new JsonContent<InteractionCallback<InteractionCallbackChoicesDataProperties>>(interactionCallback, InteractionCallbackOfInteractionCallbackChoicesDataPropertiesSerializerContext.WithOptions.InteractionCallbackInteractionCallbackChoicesDataProperties);

            case InteractionCallback<ModalProperties> interactionCallback:
                return new JsonContent<InteractionCallback<ModalProperties>>(interactionCallback, InteractionCallbackOfModalPropertiesSerializerContext.WithOptions.InteractionCallbackModalProperties);

            default:
                return new JsonContent<InteractionCallback>(this, InteractionCallbackSerializerContext.WithOptions.InteractionCallback);
        }
    }

    [JsonSerializable(typeof(InteractionCallback<InteractionMessageProperties>))]
    public partial class InteractionCallbackOfInteractionMessagePropertiesSerializerContext : JsonSerializerContext
    {
        public static InteractionCallbackOfInteractionMessagePropertiesSerializerContext WithOptions { get; } = new(Serialization.Options);
    }

    [JsonSerializable(typeof(InteractionCallback<MessageOptions>))]
    public partial class InteractionCallbackOfMessageOptionsSerializerContext : JsonSerializerContext
    {
        public static InteractionCallbackOfMessageOptionsSerializerContext WithOptions { get; } = new(Serialization.Options);
    }

    [JsonSerializable(typeof(InteractionCallback<InteractionCallbackChoicesDataProperties>))]
    public partial class InteractionCallbackOfInteractionCallbackChoicesDataPropertiesSerializerContext : JsonSerializerContext
    {
        public static InteractionCallbackOfInteractionCallbackChoicesDataPropertiesSerializerContext WithOptions { get; } = new(Serialization.Options);
    }

    [JsonSerializable(typeof(InteractionCallback<ModalProperties>))]
    public partial class InteractionCallbackOfModalPropertiesSerializerContext : JsonSerializerContext
    {
        public static InteractionCallbackOfModalPropertiesSerializerContext WithOptions { get; } = new(Serialization.Options);
    }

    [JsonSerializable(typeof(InteractionCallback))]
    public partial class InteractionCallbackSerializerContext : JsonSerializerContext
    {
        public static InteractionCallbackSerializerContext WithOptions { get; } = new(Serialization.Options);
    }
}

public class InteractionCallback<T> : InteractionCallback
{
    internal InteractionCallback(InteractionCallbackType type, T data) : base(type)
    {
        Data = data;
    }

    [JsonPropertyName("data")]
    public T Data { get; }
}
