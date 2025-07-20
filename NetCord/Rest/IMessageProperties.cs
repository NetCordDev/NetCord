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

    internal static HttpContent Serialize<TMessage>(TMessage message, JsonTypeInfo<TMessage> messageTypeInfo, IEnumerable<AttachmentProperties>? attachments)
    {
        JsonContent<TMessage> messageContent = new(message, messageTypeInfo);

        if (attachments is null || (attachments.TryGetNonEnumeratedCount(out int count) && count is 0))
            return messageContent;

        MultipartFormDataContent multipartContent = [];

        bool anyAdded = false;
        int id = 0;
        foreach (var attachment in attachments)
        {
            if (attachment.SupportsHttpSerialization)
            {
                multipartContent.Add(attachment.Serialize(), $"files[{id}]", attachment.FileName);
                anyAdded = true;
            }

            id++;
        }

        if (anyAdded)
        {
            multipartContent.Add(messageContent, "payload_json");
            return multipartContent;
        }

        return messageContent;
    }
}
