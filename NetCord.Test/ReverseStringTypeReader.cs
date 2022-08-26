using NetCord.Services.Commands;

namespace NetCord.Test;

public class ReverseStringTypeReader : CommandTypeReader<CommandContext>
{
    public override Task<object?> ReadAsync(ReadOnlyMemory<char> input, CommandContext context, CommandParameter<CommandContext> parameter, CommandServiceOptions<CommandContext> options)
    {
        var a = input.ToArray();
        Array.Reverse(a);
        return Task.FromResult((object?)new string(a));
    }
}
