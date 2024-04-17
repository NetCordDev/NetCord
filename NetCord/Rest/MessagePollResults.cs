using System.Text.Json.Serialization;

using NetCord.JsonModels;

namespace NetCord.Rest;

public partial class MessagePollResults : JsonEntity
{
    [JsonPropertyName("is_finalized")]
    public bool IsFinalized { get; set; }
    
    [JsonPropertyName("answer_counts")]
    public MessagePollAnswerCount[] AnswerCounts { get; set; }
}
