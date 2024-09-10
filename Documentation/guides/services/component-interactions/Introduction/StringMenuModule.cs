using NetCord.Services.ComponentInteractions;

namespace MyBot;

public class StringMenuModule : ComponentInteractionModule<StringMenuInteractionContext>
{
    [ComponentInteraction("menu")]
    public string Menu() => $"You selected: {string.Join(", ", Context.SelectedValues)}";
}
