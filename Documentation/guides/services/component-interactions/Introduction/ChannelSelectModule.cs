using NetCord.Services.ComponentInteractions;

namespace MyBot;

public class ChannelSelectModule : ComponentInteractionModule<ChannelSelectInteractionContext>
{
    [ComponentInteraction("select")]
    public string Select() => $"You selected: {string.Join(", ", Context.SelectedValues)}";
}
