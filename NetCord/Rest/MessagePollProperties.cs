using System.Text.Json.Serialization;

using NetCord.JsonModels;
using NetCord.Rest.JsonModels;

namespace NetCord.Rest;

public partial class MessagePollProperties : JsonEntity
{
    [JsonPropertyName("question")]
    public required JsonMessagePollMedia Question { get; set; }
    
    [JsonPropertyName("answers")]
    public required JsonMessagePollAnswer[] Answers { get; set; }
    
    [JsonPropertyName("allow_multiselect")]
    public bool AllowMultiselect { get; set; }
    
    [JsonPropertyName("layout_type")]
    public MessagePollLayoutType LayoutType { get; set; }
    
    [JsonPropertyName("duration")]
    public required int DurationInHours { get; set; }
}
