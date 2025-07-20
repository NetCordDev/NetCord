using System.Text.Json.Serialization.Metadata;

namespace NetCord.Rest;

public partial interface IMessageProperties
{
    public string? Content { get; set; }

    public IEnumerable<EmbedProperties>? Embeds { get; set; }

    public AllowedMentionsProperties? AllowedMentions { get; set; }

    public IEnumerable<AttachmentProperties>? Attachments { get; set; }

    public IEnumerable<IComponentProperties>? Components { get; set; }

    public MessageFlags? Flags { get; set; }

    internal static HttpContent Serialize<TMessage>(TMessage message, JsonTypeInfo<TMessage> messageTypeInfo) where TMessage : IMessageProperties
    {
        return Serialize(message, messageTypeInfo, message.Attachments);
    }

    internal static HttpContent Serialize<TMessage>(TMessage message, JsonTypeInfo<TMessage> messageTypeInfo, IEnumerable<AttachmentProperties>? attachments)
    {
        JsonContent<TMessage> messageContent = new(message, messageTypeInfo);

        if (attachments is null)
            return messageContent;

        MultipartFormDataContent multipartContent = new()
        {
            { messageContent, "payload_json" },
        };
        int id = 0;
        foreach (var attachment in attachments)
        {
            if (attachment.SupportsHttpSerialization)
                multipartContent.Add(attachment.Serialize(), $"files[{id}]", attachment.FileName);
            id++;
        }

        return multipartContent;
    }
}
