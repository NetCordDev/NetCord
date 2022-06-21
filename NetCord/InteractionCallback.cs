using System.Text.Json.Serialization;

namespace NetCord;

public class InteractionCallback
{
    [JsonPropertyName("type")]
    public InteractionCallbackType Type { get; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("data")]
    public object? Data { get; }

    private InteractionCallback(InteractionCallbackType type)
    {
        Type = type;
    }

    private InteractionCallback(InteractionCallbackType type, object? data)
    {
        Type = type;
        Data = data;
    }

    /// <summary>
    /// Respond to an interaction with a <paramref name="message"/>
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    public static InteractionCallback ChannelMessageWithSource(InteractionMessageProperties message)
        => new(InteractionCallbackType.ChannelMessageWithSource, message);

    /// <summary>
    /// ACK an interaction and edit a response later, the user sees a loading state
    /// </summary>
    public static InteractionCallback DeferredChannelMessageWithSource(MessageFlags? messageFlags = null)
        => new(InteractionCallbackType.DeferredChannelMessageWithSource, new InteractionMessageProperties() { Flags = messageFlags });

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
        => new(InteractionCallbackType.UpdateMessage, message);

    /// <summary>
    /// Respond to an autocomplete interaction with suggested <paramref name="choices"/>
    /// </summary>
    /// <param name="choices"></param>
    /// <returns></returns>
    public static InteractionCallback ApplicationCommandAutocompleteResult(IEnumerable<ApplicationCommandOptionChoiceProperties>? choices)
        => new(InteractionCallbackType.ApplicationCommandAutocompleteResult, new ChoicesData(choices));

    public static InteractionCallback Modal(ModalProperties modal)
        => new(InteractionCallbackType.Modal, modal);

    internal HttpContent Build()
    {
        if (Data is InteractionMessageProperties message)
        {
            MultipartFormDataContent content = new()
            {
                { new JsonContent(this), "payload_json" }
            };
            var attachments = message.Attachments;
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
        }
        else
        {
            return new JsonContent(this);
        }
    }

    private class ChoicesData
    {
        [JsonPropertyName("choices")]
        public IEnumerable<ApplicationCommandOptionChoiceProperties>? Choices { get; }

        public ChoicesData(IEnumerable<ApplicationCommandOptionChoiceProperties>? choices)
        {
            Choices = choices;
        }
    }
}