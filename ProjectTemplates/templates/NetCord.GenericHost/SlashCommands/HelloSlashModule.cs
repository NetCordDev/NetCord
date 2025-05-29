using NetCord.Rest;
using NetCord.Services;
using NetCord.Services.ApplicationCommands;

namespace NetCord.Template.Bot.SlashCommands;

/// <summary>
/// Example App (Slash) command module.
/// Usable only within guilds where bot has '<see cref="Permissions.SendMessages"/>' permission.
/// To learn more about slash commands, check out <see href="https://netcord.dev/guides/services/application-commands/introduction.html?tabs=generic-host">docs</see>.
/// </summary>
/// <param name="logger">Logger injected via DI</param>
[RequireBotPermissions<ApplicationCommandContext>(Permissions.SendMessages)]
[RequireContext<ApplicationCommandContext>(RequiredContext.Guild)]
public class HelloSlashModule(ILogger<HelloSlashModule> logger) : ApplicationCommandModule<ApplicationCommandContext>
{
    [SlashCommand(name: "hello", description: "Hello, NetCord!", DefaultGuildUserPermissions = Permissions.UseApplicationCommands)]
    public InteractionMessageProperties Hello()
    {
        logger.LogInformation("Received {Command} slash command from {User}", Context.Interaction.Data.Name, Context.User.Username);
        return new InteractionMessageProperties { Content = $"<@{Context.User.Id}> Hello from NetCord Slash commands!", AllowedMentions = AllowedMentionsProperties.All };
    }
}
