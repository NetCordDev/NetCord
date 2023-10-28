using NetCord;
using NetCord.Rest;
using NetCord.Services;
using NetCord.Services.ApplicationCommands;

namespace MyBot;

[SlashCommand("guild", "Guild command")]
public class GuildCommandsModule : ApplicationCommandModule<SlashCommandContext>
{
    [SubSlashCommand("channels", "Get guild channel count")]
    public Task ChannelsAsync()
    {
        return RespondAsync(InteractionCallback.Message($"Channels: {Context.Guild!.Channels.Count}"));
    }

    [SubSlashCommand("name", "Guild name")]
    public class GuildNameModule : ApplicationCommandModule<SlashCommandContext>
    {
        [SubSlashCommand("get", "Get guild name")]
        public Task GetNameAsync()
        {
            return RespondAsync(InteractionCallback.Message($"Name: {Context.Guild!.Name}"));
        }

        [InteractionRequireUserChannelPermissions<SlashCommandContext>(Permissions.ManageGuild)]
        [InteractionRequireBotChannelPermissions<SlashCommandContext>(Permissions.ManageGuild)]
        [SubSlashCommand("set", "Set guild name")]
        public async Task SetNameAsync(string name)
        {
            var guild = Context.Guild!;
            await guild.ModifyAsync(g => g.Name = name);
            await RespondAsync(InteractionCallback.Message($"Name: {guild.Name} -> {name}"));
        }
    }
}
