using NetCord.Services.Commands;

namespace MyBot;

public class AvatarModule : CommandModule<CommandContext>
{
    [RequireAnimatedAvatar<CommandContext>]
    [Command("avatar")]
    public string Avatar() => Context.User.GetAvatarUrl()!.ToString();
}
