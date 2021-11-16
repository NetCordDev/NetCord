using NetCord.Commands;

namespace NetCord.Test.Commands;

public class ModCommands : CommandModule<CustomCommandContext>
{
    [Command("kick")]
    public Task Kick(GuildUser user, [Remainder] string reason = null)
    {
        return user.KickAsync(reason);
    }

    [Command("ban")]
    public Task Ban(UserId userId, int? days = null, [Remainder] string reason = null)
    {
        if (days == null)
            return Context.Guild.AddBanAsync(userId, reason);
        else
            return Context.Guild.AddBanAsync(userId, (int)days, reason);
    }

    [Command("unban")]
    public Task Unban(UserId userId, [Remainder] string reason = null)
    {
        return Context.Guild.RemoveBanAsync(userId, reason);
    }
}