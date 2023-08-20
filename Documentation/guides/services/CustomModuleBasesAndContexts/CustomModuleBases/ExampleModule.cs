using NetCord;
using NetCord.Services;
using NetCord.Services.Commands;

namespace MyBot;

public class ExampleModule : CustomCommandModule
{
    [RequireContext<CommandContext>(RequiredContext.Guild)]
    [Command("color")]
    public Task ColorAsync(GuildUser? user = null)
    {
        user ??= (GuildUser)Context.User;
        var color = GetUserColor(user);
        return ReplyAsync($"#{color.RawValue:X6}");
    }
}
