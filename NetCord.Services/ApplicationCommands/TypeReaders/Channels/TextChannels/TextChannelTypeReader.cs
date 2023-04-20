namespace NetCord.Services.ApplicationCommands.TypeReaders;

public class TextChannelTypeReader<TContext> : ChannelTypeReader<TContext> where TContext : IApplicationCommandContext
{
    public override IEnumerable<ChannelType>? AllowedChannelTypes
    {
        get
        {
            yield return ChannelType.TextGuildChannel;
            yield return ChannelType.DMChannel;
            yield return ChannelType.VoiceGuildChannel;
            yield return ChannelType.GroupDMChannel;
            yield return ChannelType.AnnouncementGuildChannel;
            yield return ChannelType.AnnouncementGuildThread;
            yield return ChannelType.PublicGuildThread;
            yield return ChannelType.PrivateGuildThread;
            yield return ChannelType.StageGuildChannel;
            yield return ChannelType.DirectoryGuildChannel;
        }
    }
}
