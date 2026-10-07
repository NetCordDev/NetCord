using NetCord.Rest;
using NetCord.Services.ComponentInteractions;

namespace NetCord.Test;

public class MentionableSelectInteractions : ComponentInteractionModule<MentionableSelectInteractionContext>
{
    [ComponentInteraction("mentionables")]
    public Task MentionablesAsync()
    {
        return RespondAsync(InteractionCallback.Message($"You selected: {string.Join(", ", Context.SelectedValues)}"));
    }
}
