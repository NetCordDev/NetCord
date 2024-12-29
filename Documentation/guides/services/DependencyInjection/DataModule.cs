using NetCord.Services.Commands;

namespace MyBot;

public class DataModule(ISomeService someService) : CommandModule<CommandContext>
{
    [Command("data")]
    public string Data(int count) => string.Join(' ', someService.GetSomeData().Take(count));
}
