using NetCord.Services.ApplicationCommands;

namespace MyBot;

public class UserCommandModule : ApplicationCommandModule<UserCommandContext>
{
    [UserCommand("ID")]
    public string Id() => Context.Target.Id.ToString();
}

