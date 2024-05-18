using System.Text.Json.Serialization;

namespace NetCord.Rest;

/// <summary>
/// 
/// </summary>
/// <param name="id">Channel ID.</param>
public partial class GuildChannelPositionProperties(ulong id)
{
    /// <summary>
    /// Channel ID.
    /// </summary>
    [JsonPropertyName("id")]
    public ulong Id { get; set; } = id;

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
    /// The new parent ID for the channel that is moved.
    /// </summary>
    [JsonPropertyName("parent_id")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public ulong? ParentId { get; set; }
}
