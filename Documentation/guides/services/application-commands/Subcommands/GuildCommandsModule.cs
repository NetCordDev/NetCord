using NetCord;
using NetCord.Services;
using NetCord.Services.ApplicationCommands;

namespace MyBot;

[SlashCommand("guild", "Guild command")]
public class GuildCommandsModule : ApplicationCommandModule<ApplicationCommandContext>
{
    [SubSlashCommand("channels", "Get guild channel count")]
    public string Channels() => $"Channels: {Context.Guild!.Channels.Count}";

    [SubSlashCommand("name", "Guild name")]
    public class GuildNameModule : ApplicationCommandModule<ApplicationCommandContext>
    {
        [SubSlashCommand("get", "Get guild name")]
        public string GetName() => $"Name: {Context.Guild!.Name}";

        [RequireUserPermissions<ApplicationCommandContext>(Permissions.ManageGuild)]
        [RequireBotPermissions<ApplicationCommandContext>(Permissions.ManageGuild)]
        [SubSlashCommand("set", "Set guild name")]
        public async Task<string> SetNameAsync(string name)
        {
            var guild = Context.Guild!;
            await guild.ModifyAsync(g => g.Name = name);
            return $"Name: {guild.Name} -> {name}";
        }
    }
}
