using System.Text.Json.Serialization;

namespace NetCord.Rest;

public class GuildThreadProperties : GuildThreadFromMessageProperties
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("type")]
    public ChannelType? ChannelType { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("invitable")]
    public bool? Invitable { get; set; }

    public GuildThreadProperties(string name) : base(name)
    {
    }
}
