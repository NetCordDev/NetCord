namespace NetCord.Services.Commands.TypeReaders;

public class GroupDMChannelTypeReader<TContext> : ChannelTypeReader<TContext> where TContext : ICommandContext
{
    public override ValueTask<TypeReaderResult> ReadAsync(ReadOnlyMemory<char> input, TContext context, CommandParameter<TContext> parameter, CommandServiceConfiguration<TContext> configuration, IServiceProvider? serviceProvider)
    {
        if (context.Message.Guild is null)
        {
            var channel = context.Message.Channel;
            if (channel is not null)
                return new(GetChannel<GroupDMChannel>(channel, input.Span));
        }

        return new(TypeReaderResult.Fail("The channel was not found."));
    }
}
