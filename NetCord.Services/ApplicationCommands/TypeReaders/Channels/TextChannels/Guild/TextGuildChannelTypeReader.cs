namespace NetCord.Services.ApplicationCommands.TypeReaders;

public class TextGuildChannelTypeReader<TContext> : ChannelTypeReader<TContext> where TContext : IApplicationCommandContext
{
    public override IEnumerable<ChannelType>? AllowedChannelTypes
    {
        get
        {
            yield return ChannelType.TextGuildChannel;
            yield return ChannelType.AnnouncementGuildChannel;
        }
    }
}
