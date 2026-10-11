using NetCord.Services.ComponentInteractions;

namespace MyBot;

public class UserSelectModule : ComponentInteractionModule<UserSelectInteractionContext>
{
    [ComponentInteraction("select")]
    public string Select() => $"You selected: {string.Join(", ", Context.SelectedValues)}";
}
