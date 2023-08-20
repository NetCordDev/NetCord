using NetCord.Services.Commands;

namespace MyBot;

public class ExampleModule : CommandModule<CommandContext>
{
    [Command("average", "mean")]
    public Task AverageAsync(params double[] numbers)
    {
        return ReplyAsync($"Average: {numbers.Average()}");
    }
}
