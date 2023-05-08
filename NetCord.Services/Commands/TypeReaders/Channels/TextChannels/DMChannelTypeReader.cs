namespace NetCord.Services.Commands.TypeReaders;

public class DMChannelTypeReader<TContext> : ChannelTypeReader<TContext> where TContext : ICommandContext
{
    public override Task<object?> ReadAsync(ReadOnlyMemory<char> input, TContext context, CommandParameter<TContext> parameter, CommandServiceConfiguration<TContext> configuration, IServiceProvider? serviceProvider)
    {
        if (context.Message.Guild is null)
        {
            var channel = context.Message.Channel;
            if (channel is not null)
                return Task.FromResult<object?>(GetChannel<DMChannel>(channel, input.Span));
        }

        throw new EntityNotFoundException("The channel was not found.");
    }
}
