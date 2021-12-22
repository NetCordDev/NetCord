using NetCord.Commands;

namespace NetCord.Test.Commands;

public class ModCommands : CommandModule
{
    [Command("kick", RequiredUserPermissions = Permission.KickUsers, RequiredBotPermissions = Permission.KickUsers)]
    public static Task Kick(GuildUser user, [Remainder] string reason = null)
    {
        return user.KickAsync(new() { AuditLogReason = reason });
    }

    [Command("ban", RequiredUserPermissions = Permission.BanUsers, RequiredBotPermissions = Permission.BanUsers)]
    public Task Ban(UserId userId, int? days = null, [Remainder] string reason = null)
    {
        if (days == null)
            return Context.Guild.BanUserAsync(userId, new() { AuditLogReason = reason });
        else
            return Context.Guild.BanUserAsync(userId, (int)days, new() { AuditLogReason = reason });
    }

    [Command("unban", RequiredUserPermissions = Permission.BanUsers, RequiredBotPermissions = Permission.BanUsers)]
    public Task Unban(UserId userId, [Remainder] string reason = null)
    {
        return Context.Guild.UnbanUserAsync(userId, new() { AuditLogReason = reason });
    }

    [Command("mute", RequiredUserPermissions = Permission.ModerateUsers, RequiredBotPermissions = Permission.ModerateUsers)]
    public async Task Mute(GuildUser user, TimeSpan time, [Remainder] string reason = null)
    {
        await user.ModifyAsync(x => x.TimeOutUntil = DateTimeOffset.Now + time, new() { AuditLogReason = reason });
        await ReplyAsync($"{user} got muted");
    }

    [Command("unmute", RequiredUserPermissions = Permission.ModerateUsers, RequiredBotPermissions = Permission.ModerateUsers)]
    public async Task Unmute(GuildUser user, [Remainder] string reason = null)
    {
        await user.ModifyAsync(x => x.TimeOutUntil = default(DateTimeOffset), new() { AuditLogReason = reason });
        await ReplyAsync($"{user} got unmuted");
    }

    [Command("clear", "purge", RequiredBotChannelPermissions = Permission.ManageMessages, RequiredUserChannelPermissions = Permission.ManageMessages)]
    public async Task Clear(int count)
    {
        if (count < 1)
        {
            await ReplyAsync("To few messages!");
            return;
        }
        await Context.Client.Rest.Message.DeleteAsync(Context.Channel, GetMessagesToRemove());
        if (count == 1)
            await SendAsync("**Deleted 1 message!**");
        else
            await SendAsync($"**Deleted {count} messages!**");

        async IAsyncEnumerable<DiscordId> GetMessagesToRemove()
        {
            int i = 0;
            await foreach (var message in Context.Channel.GetMessagesAsync())
            {
                if (i == count || (DateTimeOffset.UtcNow - message.CreatedAt).TotalDays > 14)
                    break;
                yield return message;
                i++;
            }
            count = i;
        }
    }
}