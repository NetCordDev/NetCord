using System.Text.Json.Serialization;

namespace NetCord;

public class InteractionMessageProperties
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("tts")]
    public bool Tts { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("content")]
    public string? Content { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("embeds")]
    public IEnumerable<EmbedProperties>? Embeds { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("allowed_mentions")]
    public AllowedMentionsProperties? AllowedMentions { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("flags")]
    public MessageFlags? Flags { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("components")]
    public IEnumerable<ComponentProperties>? Components { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonConverter(typeof(JsonConverters.MessageAttachmentIEnumerableConverter))]
    [JsonPropertyName("attachments")]
    public IEnumerable<AttachmentProperties>? Attachments { get; set; }

    internal MultipartFormDataContent Build()
    {
        MultipartFormDataContent content = new();
        content.Add(new JsonContent(this), "payload_json");
        if (Attachments != null)
        {
            int i = 0;
            foreach (var attachment in Attachments)
            {
                content.Add(new StreamContent(attachment.Stream), $"files[{i}]", attachment.FileName);
                i++;
            }
        }
        return content;
    }

    public static implicit operator InteractionMessageProperties(string content) => new()
    {
        Content = content
    };
}