using NetCord.Commands;

namespace NetCord.Test.Commands;

public class ModCommands : CommandModule
{
    [Command("kick", RequireUserPermissions = PermissionFlags.KickMembers , RequireBotPermissions = PermissionFlags.KickMembers)]
    public static Task Kick(GuildUser user, [Remainder] string reason = null)
    {
        return user.KickAsync(reason);
    }

    [Command("ban", RequireUserPermissions = PermissionFlags.BanMembers, RequireBotPermissions = PermissionFlags.BanMembers)]
    public Task Ban(UserId userId, int? days = null, [Remainder] string reason = null)
    {
        if (days == null)
            return Context.Guild.AddBanAsync(userId, reason);
        else
            return Context.Guild.AddBanAsync(userId, (int)days, reason);
    }

    [Command("unban", RequireUserPermissions = PermissionFlags.BanMembers, RequireBotPermissions = PermissionFlags.BanMembers)]
    public Task Unban(UserId userId, [Remainder] string reason = null)
    {
        return Context.Guild.RemoveBanAsync(userId, reason);
    }
}