using NetCord;
using NetCord.Services.ComponentInteractions;

namespace MyBot;

public class ModalModule : ComponentInteractionModule<ModalInteractionContext>
{
    [ComponentInteraction("modal")]
    public string Modal() => string.Join('\n', Context.Components.OfType<LabelComponent>()
                                                                 .Select(l => l.Component)
                                                                 .OfType<TextInputComponent>()
                                                                 .Select(i => $"{i.CustomId}: {i.Value}"));
}
