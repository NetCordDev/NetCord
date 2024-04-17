using System.Text.Json.Serialization;

using NetCord.JsonModels;

namespace NetCord.Rest;

public partial class MessagePollAnswer : JsonEntity
{
    [JsonPropertyName("answer_id")]
    public ulong AnswerId { get; set; }
    
    [JsonPropertyName("poll_media")]
    public required MessagePollMedia PollMedia { get; set; }
}
