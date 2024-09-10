using NetCord.Services.ComponentInteractions;

namespace MyBot;

public class ModalModule : ComponentInteractionModule<ModalInteractionContext>
{
    [ComponentInteraction("modal")]
    public string Modal() => string.Join('\n', Context.Components.Select(c => $"{c.CustomId}: {c.Value}"));
}
