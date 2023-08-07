namespace NetCord.Services.ApplicationCommands.TypeReaders;

public class IGuildChannelTypeReader<TContext> : ChannelTypeReader<TContext> where TContext : IApplicationCommandContext
{
    public override IEnumerable<ChannelType>? AllowedChannelTypes
    {
        get
        {
            yield return ChannelType.TextGuildChannel;
            yield return ChannelType.VoiceGuildChannel;
            yield return ChannelType.CategoryChannel;
            yield return ChannelType.AnnouncementGuildChannel;
            yield return ChannelType.StageGuildChannel;
            yield return ChannelType.DirectoryGuildChannel;
            yield return ChannelType.ForumGuildChannel;
            yield return ChannelType.MediaForumGuildChannel;
        }
    }
}
