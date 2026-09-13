using NetCord.JsonModels;
using NetCord.Rest;

namespace NetCord;

internal static class ChannelFactory
{
    public static IGuildChannel CreateGuild(
        JsonChannel jsonChannel,
        RestClient client)
    => Require<IGuildChannel>(Create(jsonChannel, client));

    public static ITextChannel CreateText(
        JsonChannel jsonChannel,
        RestClient client)
    => Require<ITextChannel>(Create(jsonChannel, client));

    public static IDMChannel CreateDM(
        JsonChannel jsonChannel,
        RestClient client)
    => Require<IDMChannel>(Create(jsonChannel, client));

    public static IGroupDMChannel CreateGroupDM(
        JsonChannel jsonChannel,
        RestClient client)
     => Require<IGroupDMChannel>(Create(jsonChannel, client));

    public static IGuildThread CreateGuildThread(
        JsonChannel jsonChannel,
        RestClient client)
    => Require<IGuildThread>(Create(jsonChannel, client));

    public static IPublicGuildThread CreatePublicGuildThread(
        JsonChannel jsonChannel,
        RestClient client)
    => Require<IPublicGuildThread>(Create(jsonChannel, client));

    public static IChannel Create(
        JsonChannel jsonChannel,
        RestClient client)
    {
        return jsonChannel.Type switch
        {
            ChannelType.TextGuildChannel => new TextGuildChannel(jsonChannel, jsonChannel.GuildId.GetValueOrDefault(), client),
            ChannelType.DMChannel => new DMChannel(jsonChannel, client),
            ChannelType.VoiceGuildChannel => new VoiceGuildChannel(jsonChannel, jsonChannel.GuildId.GetValueOrDefault(), client),
            ChannelType.GroupDMChannel => new GroupDMChannel(jsonChannel, client),
            ChannelType.CategoryChannel => new CategoryGuildChannel(jsonChannel, jsonChannel.GuildId.GetValueOrDefault(), client),
            ChannelType.AnnouncementGuildChannel => new AnnouncementGuildChannel(jsonChannel, jsonChannel.GuildId.GetValueOrDefault(), client),
            ChannelType.AnnouncementGuildThread => new AnnouncementGuildThread(jsonChannel, client),
            ChannelType.PublicGuildThread => new PublicGuildThread(jsonChannel, client),
            ChannelType.PrivateGuildThread => new PrivateGuildThread(jsonChannel, client),
            ChannelType.StageGuildChannel => new StageGuildChannel(jsonChannel, jsonChannel.GuildId.GetValueOrDefault(), client),
            ChannelType.DirectoryGuildChannel => new DirectoryGuildChannel(jsonChannel, jsonChannel.GuildId.GetValueOrDefault(), client),
            ChannelType.ForumGuildChannel => new ForumGuildChannel(jsonChannel, jsonChannel.GuildId.GetValueOrDefault(), client),
            ChannelType.MediaForumGuildChannel => new MediaForumGuildChannel(jsonChannel, jsonChannel.GuildId.GetValueOrDefault(), client),
            _ => new UnknownChannel(jsonChannel, client),
        };
    }

    private static TChannel Require<TChannel>(IChannel channel)
        where TChannel : IChannel
    {
        if (channel is TChannel result)
            return result;
        
        throw new InvalidOperationException();
    }
}