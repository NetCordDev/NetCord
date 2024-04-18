using System.Text.Json.Serialization;

namespace NetCord;

public partial class MessagePollAnswerProperties
{
    [JsonPropertyName("answer_id")]
    public ulong AnswerId { get; set; }
    
    [JsonPropertyName("poll_media")]
    public MessagePollMediaProperties PollMedia { get; set; }

    public MessagePollAnswerProperties(MessagePollMediaProperties pollMedia)
    {
        PollMedia = pollMedia;
    }
}