using NetCord.Rest;
using NetCord.Services.ComponentInteractions;

namespace MyBot;

public class MentionableMenuModule : ComponentInteractionModule<MentionableMenuInteractionContext>
{
    [ComponentInteraction("menu")]
    public Task MenuAsync()
    {
        return RespondAsync(InteractionCallback.Message($"You selected: {string.Join(", ", Context.SelectedMentionables)}"));
    }
}
