using NetCord.Rest;
using NetCord.Services;
using NetCord.Services.ApplicationCommands;

namespace NetCord.Template.Bot.SlashCommands;

/// <summary>
/// Example App (Slash) command module with buttons and componentsV2.
/// To learn more about slash commands, check out <see href="https://netcord.dev/guides/services/application-commands/introduction.html?tabs=generic-host">docs</see>.
/// </summary>
/// <param name="logger">Logger injected via DI</param>
[RequireBotPermissions<ApplicationCommandContext>(Permissions.SendMessages)]
public class ButtonSlashModule(ILogger<ButtonSlashModule> logger) : ApplicationCommandModule<ApplicationCommandContext>
{
    [SlashCommand(name: "generate-button", description: "Generates message with button", DefaultGuildUserPermissions = Permissions.UseApplicationCommands)]
    public InteractionMessageProperties GenerateButton()
    {
        logger.LogInformation("Received {Command} slash command from {User}", Context.Interaction.Data.Name, Context.User.Username);
        return new InteractionMessageProperties
        {
            Components =
            [
                new ComponentContainerProperties
                {
                    AccentColor = new Color(255, 0, 0),
                    Spoiler = false,
                    Components =
                    [
                        new ComponentSectionProperties(new ComponentSectionThumbnailProperties(new ComponentMediaProperties("https://netcord.dev/images/SmallSquare.png")))
                        {
                            new TextDisplayProperties($"<@{Context.Interaction.User.Id}> Click the button to trigger HTTP Interaction"), new TextDisplayProperties("-# Or don't. It's up to you 🤖"),
                        },
                        new ComponentSeparatorProperties { Divider = true, Spacing = ComponentSeparatorSpacingSize.Small },
                        new ActionRowProperties
                        {
                            new LinkButtonProperties("https://github.com/NetCordDev/NetCord", "Check out NetCord!"), new ButtonProperties("pong-interaction", "Click for pong!", ButtonStyle.Primary)
                        }
                    ]
                }
            ],
            Flags = MessageFlags.IsComponentsV2,
            AllowedMentions = AllowedMentionsProperties.All
        };
    }
}
