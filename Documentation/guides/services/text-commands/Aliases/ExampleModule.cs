using NetCord.Services.Commands;

namespace MyBot;

public class ExampleModule : CommandModule<CommandContext>
{
    [Command("average", "mean")]
    public static string Average(params double[] numbers) => $"Average: {numbers.Average()}";
}
