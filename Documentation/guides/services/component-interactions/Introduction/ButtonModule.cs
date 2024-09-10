using NetCord.Services.ComponentInteractions;

namespace MyBot;

public class ButtonModule : ComponentInteractionModule<ButtonInteractionContext>
{
    [ComponentInteraction("button")]
    public static string Button() => "You clicked a button!";
}
