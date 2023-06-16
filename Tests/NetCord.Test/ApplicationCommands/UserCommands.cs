using NetCord.Rest;
using NetCord.Services.ApplicationCommands;

namespace NetCord.Test.ApplicationCommands;

public class UserCommands : ApplicationCommandModule<UserCommandContext>
{
    [UserCommand("Get id")]
    public Task GetId()
        => RespondAsync(InteractionCallback.ChannelMessageWithSource(Context.Target.Id.ToString()!));
}
