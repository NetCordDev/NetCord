namespace NetCord.Services.ApplicationCommands.TypeReaders;

public class CategoryGuildChannelTypeReader<TContext> : ChannelTypeReader<TContext> where TContext : IApplicationCommandContext
{
    public override IEnumerable<ChannelType>? AllowedChannelTypes
    {
        get
        {
            yield return ChannelType.CategoryChannel;
        }
    }
}
