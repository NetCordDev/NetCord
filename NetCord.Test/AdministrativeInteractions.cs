using NetCord.Rest;
using NetCord.Services;
using NetCord.Services.Interactions;

namespace NetCord.Test;

public class AdministrativeInteractions : BaseInteractionModule<ButtonInteractionContext>
{
    [RequireUserPermission<ButtonInteractionContext>(Permission.BanUsers), RequireBotPermission<ButtonInteractionContext>(Permission.BanUsers)]
    [Interaction("unban")]
    public async Task UnbanAsync(Snowflake userId)
    {
        await Context.Guild!.UnbanUserAsync(userId);
        await Context.Interaction.SendResponseAsync(InteractionCallback.ChannelMessageWithSource(new() { Content = $"**Ban cancelled by {Context.User}**", AllowedMentions = AllowedMentionsProperties.None, Components = Enumerable.Empty<ComponentProperties>() }));
    }

    [RequireUserPermission<ButtonInteractionContext>(Permission.ModerateUsers), RequireBotPermission<ButtonInteractionContext>(Permission.ModerateUsers)]
    [Interaction("unmute")]
    public async Task UnmuteAsync(Snowflake userId)
    {
        await Context.Client.Rest.ModifyGuildUserAsync(Context.Guild!, userId, u => u.TimeOutUntil = default(DateTimeOffset));
        await Context.Interaction.SendResponseAsync(InteractionCallback.ChannelMessageWithSource(new() { Content = $"**Mute cancelled by {Context.User}**", AllowedMentions = AllowedMentionsProperties.None, Components = Enumerable.Empty<ComponentProperties>() }));
    }
}