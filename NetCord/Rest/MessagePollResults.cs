using System.Text.Json.Serialization;

using NetCord.JsonModels;

namespace NetCord.Rest;

public partial class MessagePollResults : JsonEntity
{
    [JsonPropertyName("is_finalized")]
    public required bool IsFinalized { get; set; }
    
    [JsonPropertyName("answer_counts")]
    public MessagePollAnswerCount[]? Answers { get; set; }

    public bool ContainsAnswers => Answers != null;
}
