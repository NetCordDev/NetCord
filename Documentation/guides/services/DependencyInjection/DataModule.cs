using NetCord.Services.Commands;

namespace MyBot;

public class DataModule(IDataProvider dataProvider) : CommandModule<CommandContext>
{
    [Command("data")]
    public string Data(int count) => string.Join(' ', dataProvider.GetData().Take(count));
}
