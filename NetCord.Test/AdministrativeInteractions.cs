using NetCord.Services;
using NetCord.Services.Interactions;

namespace NetCord.Test;

public class AdministrativeInteractions : BaseInteractionModule<ButtonInteractionContext>
{
    [RequireUserPermission<ButtonInteractionContext>(Permission.BanUsers), RequireBotPermission<ButtonInteractionContext>(Permission.BanUsers)]
    [Interaction("unban")]
    public async Task UnbanAsync(DiscordId userId)
    {
        await Context.Guild!.UnbanUserAsync(userId);
        await Context.Interaction.SendResponseAsync(InteractionCallback.ChannelMessageWithSource(new() { Content = $"**Ban cancelled by {Context.User}**", AllowedMentions = AllowedMentionsProperties.None, Components = new() }));
    }

    [RequireUserPermission<ButtonInteractionContext>(Permission.ModerateUsers), RequireBotPermission<ButtonInteractionContext>(Permission.ModerateUsers)]
    public async Task UnmuteAsync(DiscordId userId)
    {
        await Context.Client.Rest.Guild.User.ModifyAsync(Context.Guild!, userId, u => u.TimeOutUntil = default(DateTimeOffset));
        await Context.Interaction.SendResponseAsync(InteractionCallback.ChannelMessageWithSource(new() { Content = $"**Mute cancelled by {Context.User}**", AllowedMentions = AllowedMentionsProperties.None, Components = new() }));
    }
}