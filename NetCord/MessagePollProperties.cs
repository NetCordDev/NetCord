using System.Text.Json.Serialization;

namespace NetCord;

public partial class MessagePollProperties(MessagePollMediaProperties question, IEnumerable<MessagePollAnswerProperties> answers, int durationInHours = 1)
{
    [JsonPropertyName("question")]
    public MessagePollMediaProperties Question { get; set; } = question;

    [JsonPropertyName("answers")]
    public IEnumerable<MessagePollAnswerProperties> Answers { get; set; } = answers;

    [JsonPropertyName("allow_multiselect")]
    public bool AllowMultiselect { get; set; }
    
    [JsonPropertyName("layout_type")]
    public MessagePollLayoutType? LayoutType { get; set; }

    [JsonPropertyName("duration")]
    public int DurationInHours { get; set; } = durationInHours;
}
