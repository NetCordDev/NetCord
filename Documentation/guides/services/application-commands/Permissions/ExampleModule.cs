using NetCord;
using NetCord.Rest;
using NetCord.Services.ApplicationCommands;

namespace MyBot;

public class ExampleModule : ApplicationCommandModule<ApplicationCommandContext>
{
    [SlashCommand("mention-everyone", "Mentions @everyone",
        DefaultGuildUserPermissions = Permissions.MentionEveryone,
        Contexts = [InteractionContextType.Guild])]
    public static InteractionMessageProperties MentionEveryone()
    {
        return new()
        {
            AllowedMentions = AllowedMentionsProperties.All,
            Content = "@everyone",
        };
    }
}
