using System.Text.Json.Serialization;

using NetCord.JsonModels;

namespace NetCord.Rest.JsonModels;

public partial class JsonMessagePollAnswerCount : JsonEntity
{
    [JsonPropertyName("count")]
    public int Count { get; set; }
    
    [JsonPropertyName("me_voted")]
    public bool MeVoted { get; set; }
}
