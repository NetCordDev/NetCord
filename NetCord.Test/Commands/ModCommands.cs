using NetCord.Commands;

namespace NetCord.Test.Commands;

public class ModCommands : CommandModule
{
    [Command("kick", RequiredUserPermissions = PermissionFlags.KickMembers , RequiredBotPermissions = PermissionFlags.KickMembers)]
    public static Task Kick(GuildUser user, [Remainder] string reason = null)
    {
        return user.KickAsync(reason);
    }

    [Command("ban", RequiredUserPermissions = PermissionFlags.BanMembers, RequiredBotPermissions = PermissionFlags.BanMembers)]
    public Task Ban(UserId userId, int? days = null, [Remainder] string reason = null)
    {
        if (days == null)
            return Context.Guild.AddBanAsync(userId, reason);
        else
            return Context.Guild.AddBanAsync(userId, (int)days, reason);
    }

    [Command("unban", RequiredUserPermissions = PermissionFlags.BanMembers, RequiredBotPermissions = PermissionFlags.BanMembers)]
    public Task Unban(UserId userId, [Remainder] string reason = null)
    {
        return Context.Guild.RemoveBanAsync(userId, reason);
    }

    [Command("clear", "purge", RequiredBotChannelPermissions = PermissionFlags.ManageMessages, RequiredUserChannelPermissions = PermissionFlags.ManageMessages)]
    public async Task Clear(int count)
    {
        if (count < 1)
        {
            await ReplyAsync("To few messages!");
            return;
        }
        int i = 0;
        List<DiscordId> ids = new(100);
        //List<Task> tasks = new();
        //int c = 0;
        await foreach (var message in ChannelHelper.GetMessagesAsync(Context.Client, Context.Channel))
        {
            if (i == count || (DateTimeOffset.UtcNow - message.CreatedAt).TotalDays > 14)
                break;
            //if (++c == 100)
            //{
            //    await MessageHelper.DeleteAsync(Context.Client, Context.Channel, ids);
            //    ids.Clear();
            //    c = 0;
            //}
            ids.Add(message);
            i++;
        }
        //tasks.Add(MessageHelper.DeleteAsync(Context.Client, Context.Channel, ids));
        //await Task.WhenAll(tasks);

        await MessageHelper.DeleteAsync(Context.Client, Context.Channel, ids);
        if (i == 1)
            await SendAsync("**Deleted 1 message!**");
        else
            await SendAsync($"**Deleted {i} messages!**");
    }
}