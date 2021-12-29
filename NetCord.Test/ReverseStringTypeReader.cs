using NetCord.Commands;

namespace NetCord.Test;

public class ReverseStringTypeReader : ITypeReader<CommandContext>
{
    public Task<object> ReadAsync(string input, CommandContext context, CommandParameter<CommandContext> parameter, CommandServiceOptions<CommandContext> options)
    {
        var a = input.ToCharArray();
        Array.Reverse(a);
        return Task.FromResult((object)new string(a));
    }
}