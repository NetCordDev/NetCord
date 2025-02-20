using NetCord;
using NetCord.Services.ApplicationCommands;

namespace MyBot;

[SlashCommand("permissions", "Manages permissions")]
public class PermissionsModule : ApplicationCommandModule<ApplicationCommandContext>
{
    [SubSlashCommand("user", "Manages user permissions")]
    public class UserPermissionsModule : ApplicationCommandModule<ApplicationCommandContext>
    {
        [SubSlashCommand("show", "Shows user permissions")]
        public static string Show(
            [SlashCommandParameter(Description = "The user whose permissions you want to see")] GuildUser user)
        {
            return ((GuildInteractionUser)user).Permissions.ToString();
        }
    }

    [SubSlashCommand("channel", "Manages channel permissions")]
    public class ChannelPermissionsModule : ApplicationCommandModule<ApplicationCommandContext>
    {
        [SubSlashCommand("show", "Shows channel permissions")]
        public static string Show(
            [SlashCommandParameter(Description = "The channel whose permissions you want to see")] IGuildChannel channel)
        {
            return ((IInteractionChannel)channel).Permissions.ToString();
        }
    }
}
