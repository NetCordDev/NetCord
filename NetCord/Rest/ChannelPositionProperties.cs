using System.Text.Json.Serialization;

namespace NetCord.Rest;

public partial class ChannelPositionProperties
{
    /// <summary>
    /// Channel id.
    /// </summary>
    [JsonPropertyName("id")]
    public ulong Id { get; set; }

    /// <summary>
    /// Sorting position of the channel.
    /// </summary>
    [JsonPropertyName("position")]
    public int? Position { get; set; }

    /// <summary>
    /// Syncs the permission overwrites with the new parent, if moving to a new category.
    /// </summary>
    [JsonPropertyName("lock_permissions")]
    public bool LockPermissions { get; set; }

    /// <summary>
    /// The new parent id for the channel that is moved.
    /// </summary>
    [JsonPropertyName("parent_id")]
    public ulong? ParentId { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id">Channel id.</param>
    public ChannelPositionProperties(ulong id)
    {
        Id = id;
    }

    [JsonSerializable(typeof(ChannelPositionProperties))]
    public partial class ChannelPositionPropertiesSerializerContext : JsonSerializerContext
    {
        public static ChannelPositionPropertiesSerializerContext WithOptions { get; } = new(Serialization.Options);
    }

    [JsonSerializable(typeof(IEnumerable<ChannelPositionProperties>))]
    public partial class IEnumerableOfChannelPositionPropertiesSerializerContext : JsonSerializerContext
    {
        public static IEnumerableOfChannelPositionPropertiesSerializerContext WithOptions { get; } = new(Serialization.Options);
    }
}
