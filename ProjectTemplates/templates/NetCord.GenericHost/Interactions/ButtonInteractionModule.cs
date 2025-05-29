using NetCord.Rest;
using NetCord.Services.ComponentInteractions;

namespace NetCord.Template.Bot.Interactions;

/// <summary>
/// Example Button interaction module.
/// To learn more about interactions, check out <see href="https://netcord.dev/guides/services/component-interactions/introduction.html?tabs=generic-host">docs</see>.
/// </summary>
/// <param name="logger">Logger injected via DI</param>
public class ButtonModule(ILogger<ButtonModule> logger) : ComponentInteractionModule<ButtonInteractionContext>
{
    /// <summary>
    /// Replies with deferred message to the button click.
    /// Don't forget that CustomId must match the one in the button.
    /// </summary>
    /// <returns></returns>
    [ComponentInteraction("pong-interaction")]
    public async Task PingButton()
    {
        logger.LogInformation("Received {Interaction} from {User}", Context.Interaction.Data.CustomId, Context.User.Username);
        var received = DateTime.Now;
        var callback = InteractionCallback.DeferredModifyMessage;
        await Context.Interaction.SendResponseAsync(callback).ConfigureAwait(false);

        await Task.Delay(10 * 1_000).ConfigureAwait(true); // Simulate some work

        await Context.Interaction.ModifyResponseAsync(m =>
        {
            m.Components =
            [
                new ComponentContainerProperties
                {
                    AccentColor = new Color(0, 255, 0),
                    Components =
                    [
                        new TextDisplayProperties("# Pong!"),
                        new TextDisplayProperties($"I've received your interaction at `{received:T}` and replied at `{DateTime.Now:T}`"),
                        new TextDisplayProperties("-# This was example of deferred component interaction response"),
                        new TextDisplayProperties("-# Which also used [ComponentsV2](https://discord.com/developers/docs/components/overview) over regular embeds."),
                    ]
                }
            ];
        }).ConfigureAwait(false);
    }
}
