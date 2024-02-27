using NetCord.Rest;
using NetCord.Services;
using NetCord.Services.ComponentInteractions;

namespace NetCord.Test;

public class AdministrativeInteractions : BaseComponentInteractionModule<ButtonInteractionContext>
{
    [RequireUserPermissions<ButtonInteractionContext>(Permissions.BanUsers), RequireBotPermissions<ButtonInteractionContext>(Permissions.BanUsers)]
    [ComponentInteraction("unban")]
    public async Task UnbanAsync(ulong userId)
    {
        await Context.Guild!.UnbanUserAsync(userId);
        await Context.Interaction.SendResponseAsync(InteractionCallback.Message(new() { Content = $"**Ban cancelled by {Context.User}**", AllowedMentions = AllowedMentionsProperties.None, Components = [] }));
    }

    [RequireUserPermissions<ButtonInteractionContext>(Permissions.ModerateUsers), RequireBotPermissions<ButtonInteractionContext>(Permissions.ModerateUsers)]
    [ComponentInteraction("unmute")]
    public async Task UnmuteAsync(ulong userId)
    {
        await Context.Client.Rest.ModifyGuildUserAsync(Context.Guild!.Id, userId, u => u.TimeOutUntil = default(DateTimeOffset));
        await Context.Interaction.SendResponseAsync(InteractionCallback.Message(new() { Content = $"**Mute cancelled by {Context.User}**", AllowedMentions = AllowedMentionsProperties.None, Components = [] }));
    }
}
