using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public partial class JsonMessagePollAnswerCount : JsonEntity
{
    [JsonPropertyName("count")]
    public int Count { get; set; }
    
    [JsonPropertyName("me_voted")]
    public bool MeVoted { get; set; }
}