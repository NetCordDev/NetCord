using System.Text.Json.Serialization;

using NetCord.JsonModels;

namespace NetCord.Rest;

public abstract partial class MessagePollBase : JsonEntity
{
    [JsonPropertyName("question")]
    public MessagePollMedia Question { get; set; }
    
    [JsonPropertyName("answers")]
    public MessagePollAnswer[] Answers { get; set; }
    
    [JsonPropertyName("allow_multiselect")]
    public bool AllowMultiselect { get; set; }
    
    [JsonPropertyName("layout_type")]
    public MessagePollLayoutType LayoutType { get; set; }
}
