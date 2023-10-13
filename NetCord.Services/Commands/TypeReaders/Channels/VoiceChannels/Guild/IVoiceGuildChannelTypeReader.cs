namespace NetCord.Services.Commands.TypeReaders;

public class IVoiceGuildChannelTypeReader<TContext> : ChannelTypeReader<TContext> where TContext : ICommandContext
{
    public override ValueTask<object?> ReadAsync(ReadOnlyMemory<char> input, TContext context, CommandParameter<TContext> parameter, CommandServiceConfiguration<TContext> configuration, IServiceProvider? serviceProvider)
    {
        var guild = context.Message.Guild;
        if (guild is not null)
            return new(GetGuildChannel<IVoiceGuildChannel>(guild, input.Span));

        throw new EntityNotFoundException("The channel was not found.");
    }
}
