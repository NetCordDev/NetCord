namespace NetCord.Services.ApplicationCommands.TypeReaders;

public class GroupDMChannelTypeReader<TContext> : ChannelTypeReader<TContext> where TContext : IApplicationCommandContext
{
    public override IEnumerable<ChannelType>? AllowedChannelTypes
    {
        get
        {
            yield return ChannelType.GroupDMChannel;
        }
    }
}
