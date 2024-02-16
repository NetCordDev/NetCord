using System.Text.Json.Serialization;

namespace NetCord.Rest;

public partial class GuildThreadProperties(string name) : GuildThreadFromMessageProperties(name)
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("type")]
    public ChannelType? ChannelType { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("invitable")]
    public bool? Invitable { get; set; }
}
