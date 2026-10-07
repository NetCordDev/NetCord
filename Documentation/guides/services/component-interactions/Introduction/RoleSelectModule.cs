using NetCord.Services.ComponentInteractions;

namespace MyBot;

public class RoleSelectModule : ComponentInteractionModule<RoleSelectInteractionContext>
{
    [ComponentInteraction("select")]
    public string Select() => $"You selected: {string.Join(", ", Context.SelectedValues)}";
}
