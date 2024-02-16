using System.Text.Json.Serialization;

namespace NetCord.Rest;

/// <summary>
/// 
/// </summary>
/// <param name="channelIds">Ids for channels a member is added to when the option is selected.</param>
/// <param name="roleIds">Ids for roles assigned to a member when the option is selected.</param>
/// <param name="title">Title of the option.</param>
public partial class GuildOnboardingPromptOptionProperties(IEnumerable<ulong>? channelIds, IEnumerable<ulong>? roleIds, string title)
{

    /// <summary>
    /// Id of the option.
    /// </summary>
    [JsonPropertyName("id")]
    public ulong? Id { get; set; }

    /// <summary>
    /// Ids for channels a member is added to when the option is selected.
    /// </summary>
    [JsonPropertyName("channel_ids")]
    public IEnumerable<ulong>? ChannelIds { get; set; } = channelIds;

    /// <summary>
    /// Ids for roles assigned to a member when the option is selected.
    /// </summary>
    [JsonPropertyName("role_ids")]
    public IEnumerable<ulong>? RoleIds { get; set; } = roleIds;

    /// <summary>
    /// Emoji Id of the option.
    /// </summary>
    [JsonPropertyName("emoji_id")]
    public ulong? EmojiId { get; set; }

    /// <summary>
    /// Emoji name of the option.
    /// </summary>
    [JsonPropertyName("emoji_name")]
    public string? EmojiName { get; set; }

    /// <summary>
    /// Whether the emoji is animated.
    /// </summary>
    [JsonPropertyName("emoji_animated")]
    public bool? EmojiAnimated { get; set; }

    /// <summary>
    /// Title of the option.
    /// </summary>
    [JsonPropertyName("title")]
    public string Title { get; set; } = title;

    /// <summary>
    /// 	Description of the option.
    /// </summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }
}
