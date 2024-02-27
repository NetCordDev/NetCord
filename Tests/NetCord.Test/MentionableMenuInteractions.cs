using NetCord.Rest;
using NetCord.Services.ComponentInteractions;

namespace NetCord.Test;

public class MentionableMenuInteractions : ComponentInteractionModule<MentionableMenuInteractionContext>
{
    [ComponentInteraction("mentionables")]
    public Task MentionablesAsync()
    {
        return RespondAsync(InteractionCallback.Message($"You selected: {string.Join(", ", Context.SelectedMentionables)}"));
    }
}
