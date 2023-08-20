using System.Numerics;

using NetCord.Services.Commands;

namespace MyBot;

public class ExampleModule : CommandModule<CommandContext>
{
    [Command("average")]
    public Task AverageAsync(params double[] numbers)
    {
        return ReplyAsync($"Average: {numbers.Average()}");
    }

    [Command("power")]
    public Task PowerAsync(double @base, double power = 2)
    {
        return ReplyAsync($"Result: {Math.Pow(@base, power)}");
    }

    [Command("reverse")]
    public Task ReverseAsync([CommandParameter(Remainder = true)] string toReverse)
    {
        var array = toReverse.ToCharArray();
        Array.Reverse(array);
        return ReplyAsync(new string(array));
    }

    [Command("multiply", Priority = 0)]
    public Task MultiplyAsync(int times, [CommandParameter(Remainder = true)] string text)
    {
        return ReplyAsync(string.Concat(Enumerable.Repeat(text, times)));
    }

    [Command("multiply", Priority = 1)]
    public Task MultiplyAsync(int times, BigInteger number)
    {
        return ReplyAsync((number * times).ToString());
    }
}
