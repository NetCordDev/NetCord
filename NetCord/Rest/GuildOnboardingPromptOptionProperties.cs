using System.Text.Json.Serialization;

namespace NetCord.Rest;

public partial class GuildOnboardingPromptOptionProperties
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="channelIds">Ids for channels a member is added to when the option is selected.</param>
    /// <param name="roleIds">Ids for roles assigned to a member when the option is selected.</param>
    /// <param name="title">Title of the option.</param>
    public GuildOnboardingPromptOptionProperties(IEnumerable<ulong>? channelIds, IEnumerable<ulong>? roleIds, string title)
    {
        ChannelIds = channelIds;
        RoleIds = roleIds;
        Title = title;
    }

    /// <summary>
    /// Id of the option.
    /// </summary>
    [JsonPropertyName("id")]
    public ulong? Id { get; set; }

    /// <summary>
    /// Ids for channels a member is added to when the option is selected.
    /// </summary>
    [JsonPropertyName("channel_ids")]
    public IEnumerable<ulong>? ChannelIds { get; set; }

    /// <summary>
    /// Ids for roles assigned to a member when the option is selected.
    /// </summary>
    [JsonPropertyName("role_ids")]
    public IEnumerable<ulong>? RoleIds { get; set; }

    /// <summary>
    /// Emoji of the option.
    /// </summary>
    [JsonPropertyName("emoji")]
    public EmojiProperties? Emoji { get; set; }

    /// <summary>
    /// Title of the option.
    /// </summary>
    [JsonPropertyName("title")]
    public string Title { get; set; }

    /// <summary>
    /// 	Description of the option.
    /// </summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }
}
