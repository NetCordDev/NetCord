using System.Text.Json.Serialization.Metadata;

namespace NetCord.Rest;

[GenerateMethodsForProperties]
public partial interface IMessageProperties
{
    /// <summary>
    /// The text contents of a message, limited to 2000 characters. This limit is raised to 4000 characters for Discord Nitro subscribers.
    /// </summary>
    public string? Content { get; set; }

    /// <summary>
    /// A list of up to 10 embeds to include, totalling up to 6000 characters.
    /// </summary>
    public IEnumerable<EmbedProperties>? Embeds { get; set; }

    /// <summary>
    /// Defines which users and roles the message is allowed to mention.
    /// </summary>
    public AllowedMentionsProperties? AllowedMentions { get; set; }

    /// <summary>
    /// A list of attachments to include in the message.
    /// </summary>
    public IEnumerable<AttachmentProperties>? Attachments { get; set; }

    /// <summary>
    /// A list of components to include in the message.
    /// </summary>
    public IEnumerable<IMessageComponentProperties>? Components { get; set; }

    /// <summary>
    /// Includes additional information about the message's state.
    /// </summary>
    public MessageFlags? Flags { get; set; }

    internal static HttpContent Serialize<TMessage>(TMessage message, JsonTypeInfo<TMessage> messageTypeInfo, IEnumerable<AttachmentProperties>? attachments)
    {
        JsonContent<TMessage> messageContent = new(message, messageTypeInfo);

        if (attachments is null)
            return messageContent;

        MultipartFormDataContent? multipartContent = null;

        int id = 0;
        foreach (var attachment in attachments)
        {
            if (attachment.SupportsHttpSerialization)
                (multipartContent ??= []).Add(attachment.Serialize(), $"files[{id}]", attachment.FileName);

            id++;
        }

        if (multipartContent is not null)
        {
            multipartContent.Add(messageContent, "payload_json");
            return multipartContent;
        }

        return messageContent;
    }
}
