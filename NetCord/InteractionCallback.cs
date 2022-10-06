using System.ComponentModel;
using System.Reflection.Metadata.Ecma335;
using System.Text.Json;
using System.Text.Json.Serialization;

using NetCord.Rest;

namespace NetCord;

//[JsonConverter(typeof(InteractionCallbackConverter))]
public partial class InteractionCallback
{
    [JsonPropertyName("type")]
    public InteractionCallbackType Type { get; }

    private protected InteractionCallback(InteractionCallbackType type)
    {
        Type = type;
    }

    /// <summary>
    /// Respond to an interaction with a <paramref name="message"/>
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    public static InteractionCallback ChannelMessageWithSource(InteractionMessageProperties message)
        => new InteractionCallback<InteractionMessageProperties>(InteractionCallbackType.ChannelMessageWithSource, message);

    /// <summary>
    /// ACK an interaction and edit a response later, the user sees a loading state
    /// </summary>
    public static InteractionCallback DeferredChannelMessageWithSource(MessageFlags? messageFlags = null)
        => new InteractionCallback<InteractionMessageProperties>(InteractionCallbackType.DeferredChannelMessageWithSource, new InteractionMessageProperties() { Flags = messageFlags });

    /// <summary>
    /// For components, ACK an interaction and edit the original message later; the user does not see a loading state
    /// </summary>
    public static InteractionCallback DeferredUpdateMessage
        => new(InteractionCallbackType.DeferredUpdateMessage);

    /// <summary>
    /// For components, edit the message the component was attached to
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    public static InteractionCallback UpdateMessage(InteractionMessageProperties message)
        => new InteractionCallback<InteractionMessageProperties>(InteractionCallbackType.UpdateMessage, message);

    /// <summary>
    /// Respond to an autocomplete interaction with suggested <paramref name="choices"/>
    /// </summary>
    /// <param name="choices"></param>
    /// <returns></returns>
    public static InteractionCallback ApplicationCommandAutocompleteResult(IEnumerable<ApplicationCommandOptionChoiceProperties>? choices)
        => new InteractionCallback<InteractionCallbackChoicesDataProperties>(InteractionCallbackType.ApplicationCommandAutocompleteResult, new InteractionCallbackChoicesDataProperties(choices));

    public static InteractionCallback Modal(ModalProperties modal)
        => new InteractionCallback<ModalProperties>(InteractionCallbackType.Modal, modal);

    internal HttpContent Build()
    {
        switch (Type)
        {
            case InteractionCallbackType.ChannelMessageWithSource:
            case InteractionCallbackType.DeferredChannelMessageWithSource:
            case InteractionCallbackType.UpdateMessage:
                var interactionCallback = (InteractionCallback<InteractionMessageProperties>)this;
                MultipartFormDataContent content = new()
                {
                    { new JsonContent<InteractionCallback<InteractionMessageProperties>>(interactionCallback, InteractionCallbackOfInteractionMessagePropertiesSerializerContext.WithOptions.InteractionCallbackInteractionMessageProperties), "payload_json" }
                };
                var attachments = interactionCallback.Data.Attachments;
                if (attachments != null)
                {
                    int i = 0;
                    foreach (var attachment in attachments)
                    {
                        content.Add(new StreamContent(attachment.Stream), $"files[{i}]", attachment.FileName);
                        i++;
                    }
                }
                return content;
            case InteractionCallbackType.DeferredUpdateMessage:
                return new JsonContent<InteractionCallback>(this, InteractionCallbackSerializerContext.WithOptions.InteractionCallback);
            case InteractionCallbackType.ApplicationCommandAutocompleteResult:
                return new JsonContent<InteractionCallback<InteractionCallbackChoicesDataProperties>>((InteractionCallback<InteractionCallbackChoicesDataProperties>)this, InteractionCallbackOfInteractionCallbackChoicesDataPropertiesSerializerContext.WithOptions.InteractionCallbackInteractionCallbackChoicesDataProperties);
            case InteractionCallbackType.Modal:
                return new JsonContent<InteractionCallback<ModalProperties>>((InteractionCallback<ModalProperties>)this, InteractionCallbackOfModalPropertiesSerializerContext.WithOptions.InteractionCallbackModalProperties);
            default:
                throw new InvalidEnumArgumentException(null, (int)Type, typeof(InteractionCallbackType));
        }
    }

    [JsonSerializable(typeof(InteractionCallback<InteractionMessageProperties>))]
    internal partial class InteractionCallbackOfInteractionMessagePropertiesSerializerContext : JsonSerializerContext
    {
        public static InteractionCallbackOfInteractionMessagePropertiesSerializerContext WithOptions { get; } = new(new(ToObjectExtensions._options));
    }

    [JsonSerializable(typeof(InteractionCallback))]
    internal partial class InteractionCallbackSerializerContext : JsonSerializerContext
    {
        public static InteractionCallbackSerializerContext WithOptions { get; } = new(new(ToObjectExtensions._options));
    }

    [JsonSerializable(typeof(InteractionCallback<InteractionCallbackChoicesDataProperties>))]
    internal partial class InteractionCallbackOfInteractionCallbackChoicesDataPropertiesSerializerContext : JsonSerializerContext
    {
        public static InteractionCallbackOfInteractionCallbackChoicesDataPropertiesSerializerContext WithOptions { get; } = new(new(ToObjectExtensions._options));
    }

    [JsonSerializable(typeof(InteractionCallback<ModalProperties>))]
    internal partial class InteractionCallbackOfModalPropertiesSerializerContext : JsonSerializerContext
    {
        public static InteractionCallbackOfModalPropertiesSerializerContext WithOptions { get; } = new(new(ToObjectExtensions._options));
    }

    //internal partial class InteractionCallbackConverter : JsonConverter<InteractionCallback>
    //{
    //    public override InteractionCallback? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => throw new NotImplementedException();
    //    public override void Write(Utf8JsonWriter writer, InteractionCallback value, JsonSerializerOptions options)
    //    {
    //        switch (value.Type)
    //        {
    //            case InteractionCallbackType.ChannelMessageWithSource:
    //            case InteractionCallbackType.DeferredChannelMessageWithSource:
    //            case InteractionCallbackType.UpdateMessage:
    //                JsonSerializer.Serialize(writer, (InteractionCallback<InteractionMessageProperties>)value, InteractionCallbackOfInteractionMessagePropertiesSerializerContext.WithOptions.InteractionCallbackInteractionMessageProperties);
    //                break;
    //            case InteractionCallbackType.DeferredUpdateMessage:
    //                JsonSerializer.Serialize(writer, value, InteractionCallbackSerializerContext.WithOptions.InteractionCallback);
    //                break;
    //            case InteractionCallbackType.ApplicationCommandAutocompleteResult:
    //                JsonSerializer.Serialize(writer, (InteractionCallback<ChoicesData>)value, InteractionCallbackOfChoicesDataSerializerContext.WithOptions.InteractionCallbackChoicesData);
    //                break;
    //            case InteractionCallbackType.Modal:
    //                JsonSerializer.Serialize(writer, (InteractionCallback<ModalProperties>)value, InteractionCallbackOfModalPropertiesSerializerContext.WithOptions.InteractionCallbackModalProperties);
    //                break;
    //            default:
    //                throw new InvalidEnumArgumentException(null, (int)value.Type, typeof(InteractionCallbackType));
    //        }
    //    }
    //}
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

internal partial class InteractionCallbackChoicesDataProperties
{
    [JsonPropertyName("choices")]
    public IEnumerable<ApplicationCommandOptionChoiceProperties>? Choices { get; }

    public InteractionCallbackChoicesDataProperties(IEnumerable<ApplicationCommandOptionChoiceProperties>? choices)
    {
        Choices = choices;
    }

    [JsonSerializable(typeof(InteractionCallbackChoicesDataProperties))]
    public partial class InteractionCallbackChoicesDataPropertiesSerializerContext : JsonSerializerContext
    {
        public static InteractionCallbackChoicesDataPropertiesSerializerContext WithOptions { get; } = new(new(ToObjectExtensions._options));
    }
}
