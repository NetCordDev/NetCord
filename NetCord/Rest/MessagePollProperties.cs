using System.Text.Json.Serialization;

using NetCord.JsonModels;
using NetCord.Rest.JsonModels;

namespace NetCord.Rest;

public partial class MessagePollProperties : JsonEntity
{
    [JsonPropertyName("question")]
    public JsonMessagePollMedia Question { get; set; } = null!;

    [JsonPropertyName("answers")]
    public JsonMessagePollAnswer[] Answers { get; set; } = null!;
    
    [JsonPropertyName("allow_multiselect")]
    public bool AllowMultiselect { get; set; }
    
    [JsonPropertyName("layout_type")]
    public MessagePollLayoutType LayoutType { get; set; }
    
    [JsonPropertyName("duration")]
    public int DurationInHours { get; set; }
}
