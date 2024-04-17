using System.Text.Json.Serialization;

using NetCord.JsonModels;

namespace NetCord.Rest;

public partial class MessagePollAnswerCount : JsonEntity
{
    [JsonPropertyName("id")]
    public int AnswerId { get; set; }
    
    [JsonPropertyName("count")]
    public int Count { get; set; }
    
    [JsonPropertyName("me_voted")]
    public bool MeVoted { get; set; }
}
