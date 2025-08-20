using System.Text.Json.Serialization.Metadata;

namespace NetCord.Rest;

[GenerateMethodsForProperties]
public partial interface IMessageProperties
{
    public string? Content { get; set; }

    public IEnumerable<EmbedProperties>? Embeds { get; set; }

    public AllowedMentionsProperties? AllowedMentions { get; set; }

    public IEnumerable<AttachmentProperties>? Attachments { get; set; }

    public IEnumerable<IComponentProperties>? Components { get; set; }

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
