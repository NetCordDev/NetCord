using System.Text.Json.Serialization;

namespace NetCord.Rest;

public partial class GuildChannelPositionProperties
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
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? Position { get; set; }

    /// <summary>
    /// Syncs the permission overwrites with the new parent, if moving to a new category.
    /// </summary>
    [JsonPropertyName("lock_permissions")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? LockPermissions { get; set; }

    /// <summary>
    /// The new parent id for the channel that is moved.
    /// </summary>
    [JsonPropertyName("parent_id")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public ulong? ParentId { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id">Channel id.</param>
    public GuildChannelPositionProperties(ulong id)
    {
        Id = id;
    }

    [JsonSerializable(typeof(GuildChannelPositionProperties))]
    public partial class ChannelPositionPropertiesSerializerContext : JsonSerializerContext
    {
        public static ChannelPositionPropertiesSerializerContext WithOptions { get; } = new(Serialization.Options);
    }

    [JsonSerializable(typeof(IEnumerable<GuildChannelPositionProperties>))]
    public partial class IEnumerableOfGuildChannelPositionPropertiesSerializerContext : JsonSerializerContext
    {
        public static IEnumerableOfGuildChannelPositionPropertiesSerializerContext WithOptions { get; } = new(Serialization.Options);
    }
}
