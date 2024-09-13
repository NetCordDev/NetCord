using NetCord.Services.ComponentInteractions;

namespace MyBot;

[RequireAnimatedAvatar<ButtonInteractionContext>]
public class ButtonModule : ComponentInteractionModule<ButtonInteractionContext>
{
    // All interactions here will require animated avatars
}
