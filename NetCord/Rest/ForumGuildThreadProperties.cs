using System.Text.Json.Serialization;

namespace NetCord.Rest;

public partial class ForumGuildThreadProperties : GuildThreadFromMessageProperties
{
    public ForumGuildThreadProperties(string name, ForumGuildThreadMessageProperties message) : base(name)
    {
        Message = message;
    }

    [JsonPropertyName("message")]
    public ForumGuildThreadMessageProperties Message { get; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("applied_tags")]
    public IEnumerable<ulong>? AppliedTags { get; set; }

    internal HttpContent Build()
    {
        MultipartFormDataContent content = new()
        {
            { new JsonContent<ForumGuildThreadProperties>(this, ForumGuildThreadPropertiesSerializerContext.WithOptions.ForumGuildThreadProperties), "payload_json" }
        };
        var attachments = Message.Attachments;
        if (attachments != null)
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
    }

    [JsonSerializable(typeof(ForumGuildThreadProperties))]
    public partial class ForumGuildThreadPropertiesSerializerContext : JsonSerializerContext
    {
        public static ForumGuildThreadPropertiesSerializerContext WithOptions { get; } = new(Serialization.Options);
    }
}
