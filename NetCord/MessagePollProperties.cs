using System.Text.Json.Serialization;

namespace NetCord;

public partial class MessagePollProperties
{
    [JsonPropertyName("question")]
    public MessagePollMediaProperties Question { get; set; }

    [JsonPropertyName("answers")]
    public MessagePollAnswerProperties[] Answers { get; set; }

    public MessagePollProperties(MessagePollMediaProperties question, MessagePollAnswerProperties[] answers)
    {
        Question = question;
        Answers = answers;
    }
    
    [JsonPropertyName("allow_multiselect")]
    public bool AllowMultiselect { get; set; }
    
    [JsonPropertyName("layout_type")]
    public MessagePollLayoutType LayoutType { get; set; }
    
    [JsonPropertyName("duration")]
    public int DurationInHours { get; set; }
}
