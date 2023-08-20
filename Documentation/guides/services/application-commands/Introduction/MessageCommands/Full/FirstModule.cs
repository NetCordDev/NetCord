using NetCord.Rest;
using NetCord.Services.ApplicationCommands;

namespace MyBot;

public class FirstModule : ApplicationCommandModule<MessageCommandContext>
{
    [MessageCommand("Get Length")]
    public Task GetLengthAsync()
    {
        return RespondAsync(InteractionCallback.ChannelMessageWithSource(Context.Target.Content.Length.ToString()));
    }
}
