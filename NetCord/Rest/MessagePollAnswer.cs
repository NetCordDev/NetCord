using System.Text.Json.Serialization;

using NetCord.JsonModels;

namespace NetCord.Rest;

public partial class MessagePollAnswer : JsonEntity
{
    [JsonPropertyName("poll_media")]
    public MessagePollMedia PollMedia { get; set; }
}
