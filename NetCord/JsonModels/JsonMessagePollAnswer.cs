using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public partial class JsonMessagePollAnswer : JsonEntity
{
    [JsonPropertyName("answer_id")]
    public ulong AnswerId { get; set; }
    
    [JsonPropertyName("poll_media")]
    public JsonMessagePollMedia PollMedia { get; set; }
}
