using System.Text.Json.Serialization;

using NetCord.JsonModels;

namespace NetCord.Rest.JsonModels;

public partial class JsonMessagePollResults : JsonEntity
{
    [JsonPropertyName("is_finalized")]
    public bool IsFinalized { get; set; }
    
    [JsonPropertyName("answer_counts")]
    public MessagePollAnswerCount[]? Answers { get; set; }

    public bool ContainsAnswers => Answers != null;
}
