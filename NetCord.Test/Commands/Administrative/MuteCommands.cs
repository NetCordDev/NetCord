using NetCord.Commands;

namespace NetCord.Test.Commands.Administrative;

[CommandModule(RequiredUserPermissions = Permission.ModerateUsers, RequiredBotPermissions = Permission.ModerateUsers)]
public class MuteCommands : CommandModule
{
    [Command("mute")]
    public async Task Mute(GuildUser user, TimeSpan time, [Remainder] string reason = null)
    {
        await user.ModifyAsync(x => x.TimeOutUntil = DateTimeOffset.Now + time, new() { AuditLogReason = reason });
        await ReplyAsync($"{user} got muted");
    }

    [Command("unmute")]
    public async Task Unmute(GuildUser user, [Remainder] string reason = null)
    {
        await user.ModifyAsync(x => x.TimeOutUntil = default(DateTimeOffset), new() { AuditLogReason = reason });
        await ReplyAsync($"{user} got unmuted");
    }
}