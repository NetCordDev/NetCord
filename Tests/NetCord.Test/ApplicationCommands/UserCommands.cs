using NetCord.Rest;
using NetCord.Services.ApplicationCommands;

namespace NetCord.Test.ApplicationCommands;

public class UserCommands : ApplicationCommandModule<UserCommandContext>
{
    [UserCommand("Get ID")]
    [UserCommand("Get id")]
    public Task GetId()
        => RespondAsync(InteractionCallback.Message(Context.Target.Id.ToString()!));
}
