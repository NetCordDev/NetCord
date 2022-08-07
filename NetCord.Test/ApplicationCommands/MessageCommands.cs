using NetCord.Services.ApplicationCommands;

namespace NetCord.Test.ApplicationCommands;

public class MessageCommands : ApplicationCommandModule<MessageCommandContext>
{
    //[RequireContext<MessageCommandContext>(RequiredContext.Guild)]
    [MessageCommand("Clear to this", DMPermission = false, DefaultGuildUserPermissions = Permission.ManageMessages)]
    public async Task ClearToAsync()
    {
        var now = DateTimeOffset.UtcNow;

        if (Context.Target.CreatedAt < now.AddDays(-14))
            throw new("Messages older than 2 weeks cannot be deleted");

        await RespondAsync(InteractionCallback.DeferredChannelMessageWithSource());
        int i = 0;
        var messages = Context.Channel!.GetMessagesAfterAsync(Context.Target).TakeWhile(m => m.CreatedAt < now).Select(m => { i++; return m.Id; });
        await Context.Client.Rest.DeleteMessagesAsync(Context.Channel.Id, messages);
        await Context.Interaction.ModifyResponseAsync(r => r.Content = $"**Deleted {(i == 1 ? "1 message" : $"{i} messages")}**");
    }
}