using NetCord.Services.ComponentInteractions;

namespace MyBot;

public class MentionableSelectModule : ComponentInteractionModule<MentionableSelectInteractionContext>
{
    [ComponentInteraction("select")]
    public string Select() => $"You selected: {string.Join(", ", Context.SelectedValues)}";
}
