using NetCord.Services.ComponentInteractions;

namespace MyBot;

public class UserMenuModule : ComponentInteractionModule<UserMenuInteractionContext>
{
    [ComponentInteraction("menu")]
    public string Menu() => $"You selected: {string.Join(", ", Context.SelectedUsers)}";
}
