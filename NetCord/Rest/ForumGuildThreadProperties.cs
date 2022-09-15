using System.Text.Json.Serialization;

namespace NetCord.Rest;

public class ForumGuildThreadProperties : GuildThreadFromMessageProperties
{
    public ForumGuildThreadProperties(string name, ForumGuildThreadMessageProperties message) : base(name)
    {
        Message = message;
    }

    [JsonPropertyName("message")]
    public ForumGuildThreadMessageProperties Message { get; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("applied_tags")]
    public IEnumerable<Snowflake>? AppliedTags { get; set; }

    internal HttpContent Build()
    {
        MultipartFormDataContent content = new()
        {
            { new JsonContent(this), "payload_json" }
        };
        var attachments = Message.Attachments;
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
}
