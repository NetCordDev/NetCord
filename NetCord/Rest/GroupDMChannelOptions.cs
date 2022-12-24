using System.Text.Json.Serialization;

namespace NetCord.Rest;

public partial class GroupDMChannelOptions
{
    internal GroupDMChannelOptions()
    {
    }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("icon")]
    public ImageProperties? Icon { get; set; }

    [JsonSerializable(typeof(GroupDMChannelOptions))]
    public partial class GroupDMChannelOptionsSerializerContext : JsonSerializerContext
    {
        public static GroupDMChannelOptionsSerializerContext WithOptions { get; } = new(Serialization.Options);
    }
}
