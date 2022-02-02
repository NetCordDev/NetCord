using NetCord.Services;
using NetCord.Services.SlashCommands;

namespace NetCord.Test.SlashCommands;

public class Commands : SlashCommandModule<SlashCommandContext>
{
    [SlashCommand("test", "to test")]
    public Task TestAsync(int i1, int i2, int i3, int i4 = 4, int i5 = 5, int i6 = 6)
    {
        return Context.Interaction.SendResponseAsync(InteractionCallback.ChannelMessageWithSource($"{i1} {i2} {i3} {i4} {i5} {i6}"));
    }

    [SlashCommand("search", "Search using DuckDuckGo")]
    public Task SearchAsync([Autocomplete(typeof(DDGAutocomplete))][SlashCommandParameter("Search text")] string query)
    {
        return Context.Interaction.SendResponseAsync(InteractionCallback.ChannelMessageWithSource($"https://duckduckgo.com/?q={Uri.EscapeDataString(query)}"));
    }

    [SlashCommand("percentage", "Show formatted percentage")]
    public Task PercentageAsync([TypeReader(typeof(PercentageTypeReader))] byte percentage)
    {
        return Context.Interaction.SendResponseAsync(InteractionCallback.ChannelMessageWithSource($"{percentage}%"));
    }

    [SlashCommand("ban", "Bans a user")]
    public async Task BanAsync([SlashCommandParameter("user", "User to ban")] User user, [TypeReader(typeof(DeleteMessagesDaysTypeReader))][SlashCommandParameter("delete_messages", "Delete messages")] DeleteMessagesDays deleteMessages = DeleteMessagesDays.DontRemove, string reason = "no reason")
    {
        if (Context.Guild == null)
            throw new InvalidOperationException("This command is avaible only in guild");

        await Context.Guild.BanUserAsync(user, (int)deleteMessages, new() { AuditLogReason = reason });
        await Context.Interaction.SendResponseAsync(InteractionCallback.ChannelMessageWithSource(new() { Content = $"**{user} got banned**", AllowedMentions = AllowedMentionsProperties.None }));
    }

    [SlashCommand("mute", "Mutes a user")]
    public async Task MuteAsync([SlashCommandParameter("user", "User to mute")] User user, double days, string reason = "no reason")
    {
        if (Context.Guild == null)
            throw new InvalidOperationException("This command is avaible only in guild");

        var until = DateTimeOffset.UtcNow.AddDays(days);
        await Context.Client.Rest.Guild.User.ModifyAsync(Context.Guild, user, u => u.TimeOutUntil = until, new() { AuditLogReason = reason });
        await Context.Interaction.SendResponseAsync(InteractionCallback.ChannelMessageWithSource(new() { Content = $"**{user} got muted until {new Timestamp(until, TimestampStyle.LongDateTime)}**", AllowedMentions = AllowedMentionsProperties.None }));
    }

    [SlashCommand("permissions", "Shows role permissions")]
    public Task PermissionsAsync(GuildRole role)
    {
        return Context.Interaction.SendResponseAsync(InteractionCallback.ChannelMessageWithSource(role.Permissions.ToString()));
    }

    [SlashCommand("channel-name", "Shows channel name")]
    public Task ChannelNameAsync(Channel channel)
    {
        return Context.Interaction.SendResponseAsync(InteractionCallback.ChannelMessageWithSource(channel.ToString()));
    }

    [SlashCommand("user", "Shows user info")]
    public Task UserAsync(User user)
    {
        return Context.Interaction.SendResponseAsync(InteractionCallback.ChannelMessageWithSource(user.ToString()));
    }

    [SlashCommand("add-role", "Adds role to user or users")]
    public async Task AddRole(Mentionable mentionable, [SlashCommandParameter("role", "Role to give")] GuildRole roleToAdd)
    {
        if (mentionable.Type == MentionableType.Role)
        {
            await Context.Interaction.SendResponseAsync(InteractionCallback.DeferredChannelMessageWithSource);
            var roleId = mentionable.Role.Id;
            foreach (var user in Context.Guild.Users.Values.Where(u => u.RolesIds.Contains(roleId) && !u.RolesIds.Contains(roleToAdd)))
                await user.AddRoleAsync(roleToAdd);
            await Context.Interaction.ModifyResponseAsync(x =>
            {
                x.Content = $"Role {roleToAdd} was given to users with {mentionable.Role} role";
                x.AllowedMentions = AllowedMentionsProperties.None;
            });
        } else
        {
            await ((GuildUser)mentionable.User).AddRoleAsync(roleToAdd);
            await Context.Interaction.SendResponseAsync(InteractionCallback.ChannelMessageWithSource(new() { Content = $"Role {roleToAdd} was given {mentionable.User}", AllowedMentions = AllowedMentionsProperties.None }));
        }
    }

    [SlashCommand("throw", "Throws exception")]
    public static Task ThrowAsync()
    {
        throw new();
    }

    [SlashCommand("dżejuś", "Shows dżejuś", DefaultPermission = false, GuildId = 856183259972763669, AllowedUsersIds = new ulong[] { 484036895391875093 })]
    public Task DzejusAsync()
    {
        return Context.Interaction.SendResponseAsync(InteractionCallback.ChannelMessageWithSource("https://cdn.discordapp.com/attachments/927877869173084171/937493837335646238/dzejus.gif"));
    }
}

public enum DeleteMessagesDays
{
    DontRemove = 0,
    Last24Hours = 1,
    Last2Days = 2,
    Last3Days = 3,
    Last4Days = 4,
    Last5Days = 5,
    Last6Days = 6,
    LastWeek = 7,
}