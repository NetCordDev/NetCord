using NetCord.Rest;
using NetCord.Services.Interactions;

namespace MyBot;

public class InteractionModule : InteractionModule<MentionableMenuInteractionContext>
{
    [Interaction("menu")]
    public Task MenuAsync()
    {
        return RespondAsync(InteractionCallback.ChannelMessageWithSource($"You selected: {string.Join(", ", Context.SelectedMentionables.Values)}"));
    }
}
