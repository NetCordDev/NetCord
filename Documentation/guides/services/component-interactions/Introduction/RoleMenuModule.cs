using NetCord.Services.ComponentInteractions;

namespace MyBot;

public class RoleMenuModule : ComponentInteractionModule<RoleMenuInteractionContext>
{
    [ComponentInteraction("menu")]
    public string Menu() => $"You selected: {string.Join(", ", Context.SelectedRoles)}";
}
