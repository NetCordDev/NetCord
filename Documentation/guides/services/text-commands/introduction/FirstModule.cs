using NetCord.Services.Commands;

namespace MyBot;

public class FirstModule : CommandModule<CommandContext>
{
    [Command("ping")]
    public Task PingAsync()
    {
        return ReplyAsync("pong!");
    }
}