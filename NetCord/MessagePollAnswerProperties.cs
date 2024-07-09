using System.Text.Json.Serialization;

namespace NetCord;

public partial class MessagePollAnswerProperties(MessagePollMediaProperties pollMedia)
{
    [JsonPropertyName("poll_media")]
    public MessagePollMediaProperties PollMedia { get; set; } = pollMedia;
}
