using NetCord.Rest;
using NetCord.Services.Interactions;

namespace NetCord.Test;

public class MentionableMenuInteractions : InteractionModule<MentionableMenuInteractionContext>
{
    [Interaction("mentionables")]
    public Task MentionablesAsync()
    {
        return RespondAsync(InteractionCallback.Message($"You selected: {string.Join(", ", Context.SelectedMentionables)}"));
    }
}
