using NetCord.JsonModels;
using NetCord.Rest;

namespace NetCord;

public partial interface IGuildChannel : INamedChannel
{
    public ulong GuildId { get; }
    public int? Position { get; }
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
