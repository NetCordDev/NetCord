using System.Text.Json.Serialization;

namespace NetCord.Rest;

/// <summary>
/// Represents properties used to create a guild.
/// </summary>
/// <remarks>
/// Discord deprecated application-driven guild creation in April 2025
/// and removed the corresponding API endpoint in July 2025.
/// This type is retained for compatibility with
/// <see cref="RestClient.CreateGuildAsync(GuildProperties, RestRequestProperties?, CancellationToken)"/>.
/// </remarks> 
/// <param name="name">The name of the guild.</param>
[GenerateMethodsForProperties]
[Obsolete("Discord deprecated application-driven guild creation in April 2025 and removed the corresponding API endpoint in July 2025.")]
public partial class GuildProperties(string name)
{
    /// <summary>
    /// The name of the guild.
    /// </summary>
    /// <remarks>
    /// Guild names must contain between 2 and 100 characters and cannot contain leading or trailing whitespace.
    /// </remarks>
    [JsonPropertyName("name")]
    public string Name { get; set; } = name;

    /// <summary>
    /// The icon of the guild.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("icon")]
    public ImageProperties? Icon { get; set; }

    /// <summary>
    /// The verification level required for the guild.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("verification_level")]
    public VerificationLevel? VerificationLevel { get; set; }

    /// <summary>
    /// The default message notification level of the guild.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("default_message_notifications")]
    public DefaultMessageNotificationLevel? DefaultMessageNotificationLevel { get; set; }

    /// <summary>
    /// The explicit content filter level of the guild.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("explicit_content_filter")]
    public ContentFilter? ContentFilter { get; set; }

    /// <summary>
    /// The roles to create in the guild.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("roles")]
    public IEnumerable<RoleProperties>? Roles { get; set; }

    /// <summary>
    /// The channels to create in the guild.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("channels")]
    public IEnumerable<GuildChannelProperties>? Channels { get; set; }

    /// <summary>
    /// The ID of the AFK channel.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("afk_channel_id")]
    public ulong? AfkChannelId { get; set; }

    /// <summary>
    /// The AFK timeout, in seconds.
    /// </summary>
    /// <remarks>
    /// Discord supports AFK timeouts of <c>60</c>, <c>300</c>, <c>900</c>, <c>1800</c>, and <c>3600</c> seconds.
    /// </remarks>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("afk_timeout")]
    public int? AfkTimeout { get; set; }

    /// <summary>
    /// The ID of the channel where guild notices such as welcome messages and Server Boost events are posted.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("system_channel_id")]
    public ulong? SystemChannelId { get; set; }

    /// <summary>
    /// The system channel flags of the guild.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("system_channel_flags")]
    public SystemChannelFlags? SystemChannelFlags { get; set; }
}
