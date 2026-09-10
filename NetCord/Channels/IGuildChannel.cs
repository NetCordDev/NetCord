using NetCord.JsonModels;
using NetCord.Rest;

namespace NetCord;

/// <summary>
/// Represents a channel within a guild.
/// </summary>
public partial interface IGuildChannel : INamedChannel
{
    /// <summary>
    /// The ID corresponding to the channel's parent guild.
    /// </summary>
    public ulong GuildId { get; }

    /// <summary>
    /// The channel's position within the guild channel list.
    /// </summary>
    /// <remarks>
    /// If two or more channels share a position, they are instead sorted by their ID.
    /// </remarks>
    public int? Position { get; }

    /// <summary>
    /// A list of explicit permission overwrites for specified members and roles.
    /// </summary>
    public IReadOnlyDictionary<ulong, PermissionOverwrite> PermissionOverwrites { get; }

    public static IGuildChannel CreateFromJson(JsonChannel jsonChannel, ulong guildId, RestClient client)
    {
        return jsonChannel.Type switch
        {
            ChannelType.TextGuildChannel => new TextGuildChannel(jsonChannel, guildId, client),
            ChannelType.VoiceGuildChannel => new VoiceGuildChannel(jsonChannel, guildId, client),
            ChannelType.CategoryChannel => new CategoryGuildChannel(jsonChannel, guildId, client),
            ChannelType.AnnouncementGuildChannel => new AnnouncementGuildChannel(jsonChannel, guildId, client),
            ChannelType.StageGuildChannel => new StageGuildChannel(jsonChannel, guildId, client),
            ChannelType.DirectoryGuildChannel => new DirectoryGuildChannel(jsonChannel, guildId, client),
            ChannelType.ForumGuildChannel => new ForumGuildChannel(jsonChannel, guildId, client),
            ChannelType.MediaForumGuildChannel => new MediaForumGuildChannel(jsonChannel, guildId, client),
            _ => new UnknownGuildChannel(jsonChannel, guildId, client),
        };
    }
}
