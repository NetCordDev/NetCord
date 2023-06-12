﻿using System.Text.Json.Serialization;

using NetCord.Rest;

namespace NetCord;

public partial class InteractionCallback : IHttpSerializable
{
    [JsonPropertyName("type")]
    public InteractionCallbackType Type { get; }

    private protected InteractionCallback(InteractionCallbackType type)
    {
        Type = type;
    }

    /// <summary>
    /// ACK a <see cref="InteractionType.Ping"/> interaction.
    /// </summary>
    public static InteractionCallback Pong
        => new(InteractionCallbackType.Pong);

    /// <summary>
    /// Respond to an interaction with a <paramref name="message"/>.
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    public static InteractionCallback<InteractionMessageProperties> ChannelMessageWithSource(InteractionMessageProperties message)
        => new(InteractionCallbackType.ChannelMessageWithSource, message);

    /// <summary>
    /// ACK an interaction and edit a response later, the user sees a loading state.
    /// </summary>
    public static InteractionCallback<InteractionMessageProperties> DeferredChannelMessageWithSource(MessageFlags? messageFlags = null)
        => new(InteractionCallbackType.DeferredChannelMessageWithSource, new InteractionMessageProperties() { Flags = messageFlags });

    /// <summary>
    /// For components, ACK an interaction and edit the original message later; the user does not see a loading state.
    /// </summary>
    public static InteractionCallback DeferredUpdateMessage
        => new(InteractionCallbackType.DeferredUpdateMessage);

    /// <summary>
    /// For components, edit the message the component was attached to.
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    public static InteractionCallback<InteractionMessageProperties> UpdateMessage(InteractionMessageProperties message)
        => new(InteractionCallbackType.UpdateMessage, message);

    /// <summary>
    /// Respond to an autocomplete interaction with suggested <paramref name="choices"/>.
    /// </summary>
    /// <param name="choices"></param>
    /// <returns></returns>
    public static InteractionCallback<InteractionCallbackChoicesDataProperties> ApplicationCommandAutocompleteResult(IEnumerable<ApplicationCommandOptionChoiceProperties>? choices)
        => new(InteractionCallbackType.ApplicationCommandAutocompleteResult, new InteractionCallbackChoicesDataProperties(choices));

    /// <summary>
    /// Respond to an interaction with a popup <paramref name="modal"/>.
    /// </summary>
    /// <param name="modal"></param>
    /// <returns></returns>
    public static InteractionCallback<ModalProperties> Modal(ModalProperties modal)
        => new(InteractionCallbackType.Modal, modal);

    public HttpContent Serialize()
    {
        switch (this)
        {
            case InteractionCallback<InteractionMessageProperties> interactionCallback:
                MultipartFormDataContent content = new()
                {
                    { new JsonContent<InteractionCallback<InteractionMessageProperties>>(interactionCallback, InteractionCallbackOfInteractionMessagePropertiesSerializerContext.WithOptions.InteractionCallbackInteractionMessageProperties), "payload_json" }
                };
                var attachments = interactionCallback.Data.Attachments;
                if (attachments is not null)
                {
                    int i = 0;
                    foreach (var attachment in attachments)
                    {
                        if (attachment is not GoogleCloudPlatformAttachmentProperties)
                            content.Add(new StreamContent(attachment.Stream!), $"files[{i}]", attachment.FileName);
                        i++;
                    }
                }
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
