using NetCord.Services;
using NetCord.Services.Commands;

namespace NetCord.Test.Commands.Administrative;

public class OtherCommands : CommandModule<CommandContext>
{
    [RequireUserPermissions<CommandContext>(Permissions.KickUsers), RequireBotPermissions<CommandContext>(Permissions.KickUsers)]
    public static Task Kick(GuildUser user, [Remainder] string? reason = null)
    {
        return user.KickAsync(new() { AuditLogReason = reason });
    }

    [RequireUserPermissions<CommandContext>(default, Permissions.ManageMessages), RequireBotPermissions<CommandContext>(default, Permissions.ManageMessages)]
    public async Task Clear(int count)
    {
        if (count < 1)
        {
            await ReplyAsync("To few messages!");
            return;
        }
        await Context.Client.Rest.DeleteMessagesAsync(Context.Message.ChannelId, GetMessagesToRemove());
        if (count == 1)
            await SendAsync("**Deleted 1 message!**");
        else
            await SendAsync($"**Deleted {count} messages!**");

        async IAsyncEnumerable<ulong> GetMessagesToRemove()
        {
            int i = 0;
            await foreach (var message in Context.Client.Rest.GetMessagesAsync(Context.Message.ChannelId))
            {
                if (i == count || (DateTimeOffset.UtcNow - message.CreatedAt).TotalDays > 14)
                    break;
                yield return message.Id;
                i++;
            }
            count = i;
        }
    }
}
