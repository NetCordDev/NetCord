using NetCord.Rest;
using NetCord.Services.ComponentInteractions;

namespace MyBot;

public class StringMenuModule : ComponentInteractionModule<StringMenuInteractionContext>
{
    [ComponentInteraction("menu")]
    public Task MenuAsync()
    {
        return RespondAsync(InteractionCallback.Message($"You selected: {string.Join(", ", Context.SelectedValues)}"));
    }
}
