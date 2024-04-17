using System.Text.Json.Serialization;

using NetCord.JsonModels;

namespace NetCord;

public partial class MessagePollProperties
{
    [JsonPropertyName("question")]
    public JsonMessagePollMedia Question { get; set; }

    [JsonPropertyName("answers")]
    public JsonMessagePollAnswer[] Answers { get; set; }

    public MessagePollProperties(JsonMessagePollMedia question, JsonMessagePollAnswer[] answers)
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
