namespace NetCord.Services.Commands.TypeReaders;

public class INamedChannelTypeReader<TContext> : ChannelTypeReader<TContext> where TContext : ICommandContext
{
    public override ValueTask<TypeReaderResult> ReadAsync(ReadOnlyMemory<char> input, TContext context, CommandParameter<TContext> parameter, CommandServiceConfiguration<TContext> configuration, IServiceProvider? serviceProvider)
    {
        var guild = context.Message.Guild;
        if (guild is null)
        {
            var channel = context.Message.Channel;
            if (channel is not null)
                return new(GetChannel<INamedChannel>(channel, input.Span));
        }
        else
            return new(GetGuildChannel<INamedChannel>(guild, input.Span));

        return new(TypeReaderResult.Fail("The channel was not found."));
    }
}
