namespace NetCord.Services.ApplicationCommands.TypeReaders;

public class GuildThreadTypeReader<TContext> : ChannelTypeReader<TContext> where TContext : IApplicationCommandContext
{
    public override IEnumerable<ChannelType>? AllowedChannelTypes
    {
        get
        {
            yield return ChannelType.AnnouncementGuildThread;
            yield return ChannelType.PublicGuildThread;
            yield return ChannelType.PrivateGuildThread;
        }
    }
}
