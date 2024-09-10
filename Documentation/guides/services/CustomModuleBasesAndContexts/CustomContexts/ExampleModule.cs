using NetCord.Services;
using NetCord.Services.Commands;

namespace MyBot;

public class ExampleModule : CommandModule<CustomCommandContext>
{
    [RequireContext<CustomCommandContext>(RequiredContext.Guild)]
    [Command("botname")]
    public string BotName()
    {
        var user = Context.BotGuildUser;
        return user.Nickname ?? user.Username;
    }
}
