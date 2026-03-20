using NetCord.Services.Commands;

namespace NetCord.Test;

public class ReverseStringTypeReader : CommandTypeParser<CommandContext>
{
    public override ValueTask<CommandTypeParserResult> ParseAsync(ReadOnlyMemory<char> input, CommandContext context, CommandParameter<CommandContext> parameter, CommandServiceConfiguration<CommandContext> configuration, IServiceProvider? serviceProvider)
    {
        var a = input.ToArray();
        Array.Reverse(a);
        return new(CommandTypeParserResult.Success(new string(a)));
    }
}
