using System.Text.Json.Serialization;

namespace NetCord;

public partial class MessagePollProperties(MessagePollMediaProperties question, IEnumerable<MessagePollAnswerProperties> answers, int durationInHours)
{
    [JsonPropertyName("question")]
    public MessagePollMediaProperties Question { get; set; } = question;

    [JsonPropertyName("answers")]
    public IEnumerable<MessagePollAnswerProperties> Answers { get; set; } = answers;

    [JsonPropertyName("duration")]
    public int DurationInHours { get; set; } = durationInHours;

    [JsonPropertyName("allow_multiselect")]
    public bool AllowMultiselect { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("layout_type")]
    public MessagePollLayoutType? LayoutType { get; set; }
}
