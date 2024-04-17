using System.Text.Json.Serialization;

using NetCord.JsonModels;

namespace NetCord.Rest;

public partial class MessagePollAnswer : JsonEntity
{
    [JsonPropertyName("answer_id")]
    public int? Id { get; set; }
    
    [JsonPropertyName("poll_media")]
    public MessagePollMedia PollMedia { get; set; }
}
