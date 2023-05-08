namespace NetCord.Services.Commands.TypeReaders;

public class TextChannelTypeReader<TContext> : ChannelTypeReader<TContext> where TContext : ICommandContext
{
    public override Task<object?> ReadAsync(ReadOnlyMemory<char> input, TContext context, CommandParameter<TContext> parameter, CommandServiceConfiguration<TContext> configuration, IServiceProvider? serviceProvider)
    {
        var guild = context.Message.Guild;
        if (guild is null)
        {
            var channel = context.Message.Channel;
            if (channel is not null)
                return Task.FromResult<object?>(GetChannel<TextChannel>(channel, input.Span));
        }
        else
            return Task.FromResult<object?>(GetGuildChannel<TextChannel>(guild, input.Span));

        throw new EntityNotFoundException("The channel was not found.");
    }
}
