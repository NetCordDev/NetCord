namespace NetCord.Services.Commands.TypeReaders;

public class GroupDMChannelTypeReader<TContext> : ChannelTypeReader<TContext> where TContext : ICommandContext
{
    public override Task<object?> ReadAsync(ReadOnlyMemory<char> input, TContext context, CommandParameter<TContext> parameter, CommandServiceConfiguration<TContext> configuration)
    {
        if (context.Message.Guild is null)
        {
            var channel = context.Message.Channel;
            if (channel is not null)
                return Task.FromResult<object?>(GetChannel<GroupDMChannel>(channel, input.Span));
        }

        throw new EntityNotFoundException("The channel was not found.");
    }
}
