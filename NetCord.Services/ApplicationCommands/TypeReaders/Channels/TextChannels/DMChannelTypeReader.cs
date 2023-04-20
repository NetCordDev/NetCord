namespace NetCord.Services.ApplicationCommands.TypeReaders;

public class DMChannelTypeReader<TContext> : ChannelTypeReader<TContext> where TContext : IApplicationCommandContext
{
    public override IEnumerable<ChannelType>? AllowedChannelTypes
    {
        get
        {
            yield return ChannelType.DMChannel;
            yield return ChannelType.GroupDMChannel;
        }
    }
}
