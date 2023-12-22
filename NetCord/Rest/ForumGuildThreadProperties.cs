using System.Text.Json.Serialization;

namespace NetCord.Rest;

public partial class ForumGuildThreadProperties : GuildThreadFromMessageProperties, IHttpSerializable
{
    public ForumGuildThreadProperties(string name, ForumGuildThreadMessageProperties message) : base(name)
    {
        Message = message;
    }

    [JsonPropertyName("message")]
    public ForumGuildThreadMessageProperties Message { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("applied_tags")]
    public IEnumerable<ulong>? AppliedTags { get; set; }

    public HttpContent Serialize()
    {
        MultipartFormDataContent content = new()
        {
            { new JsonContent<ForumGuildThreadProperties>(this, Serialization.Default.ForumGuildThreadProperties), "payload_json" },
        };
        AttachmentProperties.AddAttachments(content, Message.Attachments);
        return content;
    }
}
