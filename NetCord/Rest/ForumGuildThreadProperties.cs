using System.Text.Json.Serialization;

namespace NetCord.Rest;

public partial class ForumGuildThreadProperties(string name, ForumGuildThreadMessageProperties message) : GuildThreadFromMessageProperties(name), IHttpSerializable
{
    [JsonPropertyName("message")]
    public ForumGuildThreadMessageProperties Message { get; set; } = message;

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
