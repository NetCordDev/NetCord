namespace NetCord.Services.Commands.TypeReaders;

public class DirectoryGuildChannelTypeReader<TContext> : ChannelTypeReader<TContext> where TContext : ICommandContext
{
    public override ValueTask<CommandTypeParserResult> ParseAsync(ReadOnlyMemory<char> input, TContext context, CommandParameter<TContext> parameter, CommandServiceConfiguration<TContext> configuration, IServiceProvider? serviceProvider)
    {
        var guild = context.Message.Guild;
        if (guild is not null)
            return new(GetGuildChannel<DirectoryGuildChannel>(guild, input.Span));

        return new(CommandTypeParserResult.Fail("The channel was not found."));
    }
}
