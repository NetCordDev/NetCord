using NetCord.Services.Commands;

namespace NetCord.Test;

public class ReverseStringTypeReader : CommandTypeReader<CommandContext>
{
    public override Task<object?> ReadAsync(string input, CommandContext context, CommandParameter<CommandContext> parameter, CommandServiceOptions<CommandContext> options)
    {
        var a = input.ToCharArray();
        Array.Reverse(a);
        return Task.FromResult((object?)new string(a));
    }
}