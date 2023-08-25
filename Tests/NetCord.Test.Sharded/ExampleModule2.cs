using NetCord.Rest;
using NetCord.Services.ApplicationCommands;

namespace NetCord.Test.Sharded;

[SlashCommand("xd", "Ping")]
public class ExampleModule2 : ApplicationCommandModule<SlashCommandContext>
{
    [SubSlashCommand("xd2", "Ping")]
    public class ExampleModule22 : ApplicationCommandModule<SlashCommandContext>
    {
        [SubSlashCommand("xd3", "Ping")]
        public Task PingAsync([SlashCommandParameter(Name = "guild_user_flags")] GuildUserFlags guildUserFlags)
        {
            return RespondAsync(InteractionCallback.ChannelMessageWithSource(guildUserFlags.ToString()));
        }

        [SubSlashCommand("permissions", "Permissions")]
        public Task PermissionsAsync(Permissions permissions)
        {
            return RespondAsync(InteractionCallback.ChannelMessageWithSource(permissions.ToString()));
        }
    }

    [SubSlashCommand("xd4", "Ping")]
    public Task PingAsync(E? e)
    {
        return RespondAsync(InteractionCallback.ChannelMessageWithSource(e.GetValueOrDefault().ToString()));
    }
}

public enum E
{
    A,
    B,
    C,
}
