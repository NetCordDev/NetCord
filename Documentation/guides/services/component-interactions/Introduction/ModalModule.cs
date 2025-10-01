using NetCord;
using NetCord.Services.ComponentInteractions;

namespace MyBot;

public class ModalModule : ComponentInteractionModule<ModalInteractionContext>
{
    [ComponentInteraction("modal")]
    public string Modal() => string.Join('\n', Context.Components.OfType<Label>()
                                                                 .Select(l => l.Component)
                                                                 .OfType<TextInput>()
                                                                 .Select(i => $"{i.CustomId}: {i.Value}"));
}
