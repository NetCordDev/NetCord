using NetCord.Services.ComponentInteractions;

namespace MyBot;

public class StringSelectModule : ComponentInteractionModule<StringSelectInteractionContext>
{
    [ComponentInteraction("select")]
    public string Select() => $"You selected: {string.Join(", ", Context.Values)}";
}
