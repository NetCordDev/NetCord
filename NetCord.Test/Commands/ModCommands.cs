using NetCord.Commands;

namespace NetCord.Test.Commands;

public class ModCommands : CommandModule
{
    [Command("kick", RequiredUserPermissions = Permission.KickMembers , RequiredBotPermissions = Permission.KickMembers)]
    public static Task Kick(GuildUser user, [Remainder] string reason = null)
    {
        return user.KickAsync(reason);
    }

    [Command("ban", RequiredUserPermissions = Permission.BanMembers, RequiredBotPermissions = Permission.BanMembers)]
    public Task Ban(UserId userId, int? days = null, [Remainder] string reason = null)
    {
        if (days == null)
            return Context.Guild.AddBanAsync(userId, reason);
        else
            return Context.Guild.AddBanAsync(userId, (int)days, reason);
    }

    [Command("unban", RequiredUserPermissions = Permission.BanMembers, RequiredBotPermissions = Permission.BanMembers)]
    public Task Unban(UserId userId, [Remainder] string reason = null)
    {
        return Context.Guild.RemoveBanAsync(userId, reason);
    }

    [Command("clear", "purge", RequiredBotChannelPermissions = Permission.ManageMessages, RequiredUserChannelPermissions = Permission.ManageMessages)]
    public async Task Clear(int count)
    {
        if (count < 1)
        {
            await ReplyAsync("To few messages!");
            return;
        }
        await MessageHelper.DeleteAsync(Context.Client, Context.Channel, GetMessagesToRemove());
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