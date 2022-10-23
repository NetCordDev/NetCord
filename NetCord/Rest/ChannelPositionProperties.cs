using System.Text.Json.Serialization;

namespace NetCord.Rest;

public partial class ChannelPositionProperties
{
    [JsonPropertyName("id")]
    public ulong Id { get; }

    [JsonPropertyName("position")]
    public int? Position { get; set; }

    [JsonPropertyName("lock_permissions")]
    public bool? LockPermissions { get; set; }

    [JsonPropertyName("parent_id")]
    public ulong? ParentId { get; set; }

    public ChannelPositionProperties(ulong id)
    {
        Id = id;
    }

    [JsonSerializable(typeof(ChannelPositionProperties))]
    public partial class ChannelPositionPropertiesSerializerContext : JsonSerializerContext
    {
        public static ChannelPositionPropertiesSerializerContext WithOptions { get; } = new(new(ToObjectExtensions._options));
    }

    [JsonSerializable(typeof(IEnumerable<ChannelPositionProperties>))]
    public partial class IEnumerableOfChannelPositionPropertiesSerializerContext : JsonSerializerContext
    {
        public static IEnumerableOfChannelPositionPropertiesSerializerContext WithOptions { get; } = new(new(ToObjectExtensions._options));
    }
}
