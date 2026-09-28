using System.Text.Json.Serialization;

namespace NetCord.Rest;

/// <summary>
/// Represents options for modifying a guild member.
/// </summary>
[GenerateMethodsForProperties]
public partial class GuildUserOptions : CurrentGuildUserOptions
{
    internal GuildUserOptions()
    {
    }

    /// <summary>
    /// The IDs of the roles assigned to the member.
    /// </summary>
    /// <remarks>
    /// Modifying the member's roles requires the <c>MANAGE_ROLES</c> permission.
    /// </remarks>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("roles")]
    public IEnumerable<ulong>? RoleIds { get; set; }

    /// <summary>
    /// Whether the member should be muted in voice channels.
    /// </summary>
    /// <remarks>
    /// Requires the <c>MUTE_MEMBERS</c> permission. Discord returns a 400 response if the member is not connected to a voice channel.
    /// </remarks>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("mute")]
    public bool? Muted { get; set; }

    /// <summary>
    /// Whether the member should be deafened in voice channels.
    /// </summary>
    /// <remarks>
    /// Requires the <c>DEAFEN_MEMBERS</c> permission. Discord returns a 400 response if the member is not connected to a voice channel.
    /// </remarks>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("deaf")]
    public bool? Deafened { get; set; }

    /// <summary>
    /// The ID of the voice channel to move the member to.
    /// </summary>
    /// <remarks>
    /// Moving a member requires the <c>MOVE_MEMBERS</c> permission and permission to connect to the target channel.
    /// </remarks>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("channel_id")]
    public ulong? ChannelId { get; set; }

    /// <summary>
    /// The time at which the member's communication timeout should expire.
    /// </summary>
    /// <remarks>
    /// The timeout may be up to 28 days in the future and requires the <c>MODERATE_MEMBERS</c> permission.
    /// Discord rejects attempts to time out the guild owner or a member with the <c>ADMINISTRATOR</c> permission.
    /// </remarks>
    [JsonConverter(typeof(JsonConverters.NullableDateTimeOffsetConverter))]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("communication_disabled_until")]
    public DateTimeOffset? TimeOutUntil { get; set; }

    /// <summary>
    /// The guild member flags to set.
    /// </summary>
    /// <remarks>
    /// Only editable guild member flags can be changed. Discord currently marks
    /// <c>BYPASSES_VERIFICATION</c> as editable.
    /// </remarks>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("flags")]
    public GuildUserFlags? GuildFlags { get; set; }
}
