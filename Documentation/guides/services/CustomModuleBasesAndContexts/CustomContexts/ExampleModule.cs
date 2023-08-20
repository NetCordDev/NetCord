using NetCord.Services;
using NetCord.Services.Commands;

namespace MyBot;

public class ExampleModule : CommandModule<CustomCommandContext>
{
    [RequireContext<CustomCommandContext>(RequiredContext.Guild)]
    [Command("botname")]
    public Task BotNameAsync()
    {
        var user = Context.BotGuildUser;
        return ReplyAsync(user.Nickname ?? user.Username);
    }
}
