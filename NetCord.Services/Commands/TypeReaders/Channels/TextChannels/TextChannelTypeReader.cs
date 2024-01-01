namespace NetCord.Services.Commands.TypeReaders;

public class TextChannelTypeReader<TContext> : ChannelTypeReader<TContext> where TContext : ICommandContext
{
    public override ValueTask<TypeReaderResult> ReadAsync(ReadOnlyMemory<char> input, TContext context, CommandParameter<TContext> parameter, CommandServiceConfiguration<TContext> configuration, IServiceProvider? serviceProvider)
    {
        var guild = context.Message.Guild;
        if (guild is null)
        {
            var channel = context.Message.Channel;
            if (channel is not null)
                return new(GetChannel<TextChannel>(channel, input.Span));
        }
        else
            return new(GetGuildChannel<TextChannel>(guild, input.Span));

        return new(TypeReaderResult.Fail("The channel was not found."));
    }
}
