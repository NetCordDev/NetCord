using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public class JsonMessagePollAnswer
{
    [JsonPropertyName("answer_id")]
    public uint AnswerId { get; set; }
    
    [JsonPropertyName("poll_media")]
    public JsonMessagePollMedia PollMedia { get; set; }
}
