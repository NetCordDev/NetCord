using NetCord.Rest;
using NetCord.Services.ApplicationCommands;

namespace MyBot;

public class FirstModule : ApplicationCommandModule<UserCommandContext>
{
    [UserCommand("Username")]
    public Task UsernameAsync()
    {
        return RespondAsync(InteractionCallback.ChannelMessageWithSource(Context.Target.Username));
    }
}
