using NetCord.Services;
using NetCord.Services.Commands;

namespace NetCord.Test;

public class ReverseStringTypeReader : CommandTypeReader<CommandContext>
{
    public override ValueTask<TypeReaderResult> ReadAsync(ReadOnlyMemory<char> input, CommandContext context, CommandParameter<CommandContext> parameter, CommandServiceConfiguration<CommandContext> configuration, IServiceProvider? serviceProvider)
    {
        var a = input.ToArray();
        Array.Reverse(a);
        return new(TypeReaderResult.Success(new string(a)));
    }
}
