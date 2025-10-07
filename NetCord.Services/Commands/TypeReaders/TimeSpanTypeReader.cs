using NetCord.Services.Helpers;

namespace NetCord.Services.Commands.TypeReaders;

public partial class TimeSpanTypeReader<TContext> : CommandTypeParser<TContext> where TContext : ICommandContext
{
    public override ValueTask<CommandTypeParserResult> ParseAsync(ReadOnlyMemory<char> input, TContext context, CommandParameter<TContext> parameter, CommandServiceConfiguration<TContext> configuration, IServiceProvider? serviceProvider)
    {
        return new(TimeSpanTypeReaderHelper.Read(input.ToString(), configuration.IgnoreCase, configuration.CultureInfo) is { } value ? CommandTypeParserResult.Success(value) : CommandTypeParserResult.ParseFail(parameter.Name));
    }
}
