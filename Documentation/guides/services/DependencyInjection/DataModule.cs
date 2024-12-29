using NetCord.Services.Commands;

namespace MyBot;

public class DataModule(IDataProvider provider) : CommandModule<CommandContext>
{
    [Command("data")]
    public string Data(int count) => string.Join(' ', provider.GetData().Take(count));
}
