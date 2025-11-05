namespace NetCord.Services.Commands.TypeReaders;

public class ReadOnlyMemoryOfCharTypeReader<TContext> : CommandTypeParser<TContext> where TContext : ICommandContext
{
    public override ValueTask<CommandTypeParserResult> ParseAsync(ReadOnlyMemory<char> input, TContext context, CommandParameter<TContext> parameter, CommandServiceConfiguration<TContext> configuration, IServiceProvider? serviceProvider) => new(CommandTypeParserResult.Success(input));
}
