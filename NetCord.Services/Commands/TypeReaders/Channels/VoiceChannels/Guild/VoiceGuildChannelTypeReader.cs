namespace NetCord.Services.Commands.TypeReaders;

public class VoiceGuildChannelTypeReader<TContext> : ChannelTypeReader<TContext> where TContext : ICommandContext
{
    public override ValueTask<CommandTypeParserResult> ParseAsync(ReadOnlyMemory<char> input, TContext context, CommandParameter<TContext> parameter, CommandServiceConfiguration<TContext> configuration, IServiceProvider? serviceProvider)
    {
        var guild = context.Message.Guild;
        if (guild is not null)
            return new(GetGuildChannel<VoiceGuildChannel>(guild, input.Span));

        return new(CommandTypeParserResult.Fail("The channel was not found."));
    }
}
