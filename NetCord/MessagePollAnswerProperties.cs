using System.Text.Json.Serialization;

namespace NetCord;

public partial class MessagePollAnswerProperties(MessagePollMediaProperties pollMedia)
{
    [JsonPropertyName("answer_id")]
    public ulong AnswerId { get; set; }
    
    [JsonPropertyName("poll_media")]
    public MessagePollMediaProperties PollMedia { get; set; } = pollMedia;
}
