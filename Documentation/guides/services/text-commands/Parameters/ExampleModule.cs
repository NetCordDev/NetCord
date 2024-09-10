using System.Numerics;

using NetCord.Services.Commands;

namespace MyBot;

public class ExampleModule : CommandModule<CommandContext>
{
    [Command("average")]
    public static string Average(params double[] numbers) => $"Average: {numbers.Average()}";

    [Command("power")]
    public static string Power(double @base, double power = 2) => $"Result: {Math.Pow(@base, power)}";

    [Command("reverse")]
    public static string Reverse([CommandParameter(Remainder = true)] string toReverse)
    {
        var array = toReverse.ToCharArray();
        Array.Reverse(array);
        return new string(array);
    }

    [Command("multiply", Priority = 0)]
    public static string Multiply(int times, [CommandParameter(Remainder = true)] string text)
    {
        return string.Concat(Enumerable.Repeat(text, times));
    }

    [Command("multiply", Priority = 1)]
    public static string Multiply(int times, BigInteger number) => (number * times).ToString();
}
