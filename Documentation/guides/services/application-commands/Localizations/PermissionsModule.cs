using NetCord;
using NetCord.Services.ApplicationCommands;

namespace MyBot;

[SlashCommand("permissions", "Manages permissions")]
public class PermissionsModule : ApplicationCommandModule<SlashCommandContext>
{
    [SubSlashCommand("user", "Manages user permissions")]
    public class UserPermissionsModule : ApplicationCommandModule<SlashCommandContext>
    {
        [SubSlashCommand("show", "Shows user permissions")]
        public static string Show(
            [SlashCommandParameter(Description = "The user whose permissions you want to see")] GuildUser user)
        {
            return ((GuildInteractionUser)user).Permissions.ToString();
        }
    }

    [SubSlashCommand("role", "Manages role permissions")]
    public class RolePermissionsModule : ApplicationCommandModule<SlashCommandContext>
    {
        [SubSlashCommand("show", "Shows role permissions")]
        public static string Show(
            [SlashCommandParameter(Description = "The channel whose permissions you want to see")] IGuildChannel channel)
        {
            return ((IInteractionChannel)channel).Permissions.ToString();
        }
    }
}
