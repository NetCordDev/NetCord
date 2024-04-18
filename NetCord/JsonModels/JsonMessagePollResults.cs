using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public class JsonMessagePollResults : JsonEntity
{
    [JsonPropertyName("is_finalized")]
    public bool IsFinalized { get; set; }
    
    [JsonPropertyName("answer_counts")]
    public MessagePollAnswerCount[]? Answers { get; set; }
}
