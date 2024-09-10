using NetCord.Services.ComponentInteractions;

namespace MyBot;

public class ChannelMenuModule : ComponentInteractionModule<ChannelMenuInteractionContext>
{
    [ComponentInteraction("menu")]
    public string Menu() => $"You selected: {string.Join(", ", Context.SelectedChannels)}";
}
