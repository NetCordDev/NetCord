using NetCord.Rest;
using NetCord.Services.ApplicationCommands;

namespace NetCord.Test.ApplicationCommands;

public class MessageCommands : ApplicationCommandModule<MessageCommandContext>
{
    [MessageCommand("Clear", Contexts = [InteractionContextType.Guild], DefaultGuildUserPermissions = Permissions.ManageMessages)]
    [MessageCommand("Clear to this", Contexts = [InteractionContextType.Guild], DefaultGuildUserPermissions = Permissions.ManageMessages)]
    public async Task ClearToAsync()
    {
        var now = DateTimeOffset.UtcNow;

        if (Context.Target.CreatedAt < now.AddDays(-14))
            throw new("Messages older than 2 weeks cannot be deleted");

        await RespondAsync(InteractionCallback.DeferredMessage());
        int i = 0;
        var messages = Context.Channel!.GetMessagesAsync(new() { From = Context.Target.Id, Direction = PaginationDirection.After }).TakeWhile(m => m.CreatedAt < now).Select(m => { i++; return m.Id; });
        await Context.Client.Rest.DeleteMessagesAsync(Context.Channel.Id, messages);
        await Context.Interaction.ModifyResponseAsync(r => r.Content = $"**Deleted {(i == 1 ? "1 message" : $"{i} messages")}**");
    }
}
