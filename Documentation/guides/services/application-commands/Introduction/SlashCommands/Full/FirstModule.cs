using NetCord.Rest;
using NetCord.Services.ApplicationCommands;

namespace MyBot;

public class FirstModule : ApplicationCommandModule<SlashCommandContext>
{
    [SlashCommand("ping", "This is a ping command")]
    public Task PingAsync()
    {
        return RespondAsync(InteractionCallback.Message("pong!"));
    }
}
