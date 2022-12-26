using NetCord.Rest;
using NetCord.Services;
using NetCord.Services.Interactions;

namespace NetCord.Test;

public class AdministrativeInteractions : BaseInteractionModule<ButtonInteractionContext>
{
    [RequireUserPermissions<ButtonInteractionContext>(Permissions.BanUsers), RequireBotPermissions<ButtonInteractionContext>(Permissions.BanUsers)]
    [Interaction("unban")]
    public async Task UnbanAsync(ulong userId)
    {
        await Context.Guild!.UnbanUserAsync(userId);
        await Context.Interaction.SendResponseAsync(InteractionCallback.ChannelMessageWithSource(new() { Content = $"**Ban cancelled by {Context.User}**", AllowedMentions = AllowedMentionsProperties.None, Components = Enumerable.Empty<ComponentProperties>() }));
    }

    [RequireUserPermissions<ButtonInteractionContext>(Permissions.ModerateUsers), RequireBotPermissions<ButtonInteractionContext>(Permissions.ModerateUsers)]
    [Interaction("unmute")]
    public async Task UnmuteAsync(ulong userId)
    {
        await Context.Client.Rest.ModifyGuildUserAsync(Context.Guild!.Id, userId, u => u.TimeOutUntil = default(DateTimeOffset));
        await Context.Interaction.SendResponseAsync(InteractionCallback.ChannelMessageWithSource(new() { Content = $"**Mute cancelled by {Context.User}**", AllowedMentions = AllowedMentionsProperties.None, Components = Enumerable.Empty<ComponentProperties>() }));
    }
}
