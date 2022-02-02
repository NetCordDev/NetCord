using System.Text.Json.Serialization;

namespace NetCord;

public class ThreadProperties : ThreadWithMessageProperties
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("type")]
    public ChannelType? ChannelType { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("invitable")]
    public bool? Invitable { get; set; }

    public ThreadProperties(string name) : base(name)
    {
    }
}